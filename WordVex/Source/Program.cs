using System.Text.Json;

string text = File.ReadAllText("Resources/Text.txt").ToLower();
string[] words = text.Split(' ');

List<string> uniqueWords = [];

Dictionary<string, Dictionary<string, float>> vectors = [];

foreach (string word in words)
{
    if (!uniqueWords.Contains(word))
    {
        uniqueWords.Add(word);
        vectors.Add(word, new Dictionary<string, float>());
    }
}

foreach (var vector in vectors)
{
    foreach (string uniqueWord in uniqueWords)
    {
        if (vector.Key != uniqueWord)
        {
            vector.Value.Add(uniqueWord, 0);
        }
    }
}

for (int i = 0; i < words.Length; i++)
{
    string currentWord = words[i];

    if (i > 0)
    {
        for (int j = 1; j <= i; j++)
        {
            string previousWord = words[i - j];
            if (currentWord != previousWord)
            {
                vectors[currentWord][previousWord] += 1F / j;
            }
        }
    }

    if (i < words.Length - 1)
    {
        for (int j = 1; j <= words.Length - i - 1; j++)
        {
            string nextWord = words[i + j];
            if (currentWord != nextWord)
            {
                vectors[currentWord][nextWord] += 1F / j;
            }
        }
    }
}

foreach (var vector in vectors)
{
    float sum = vector.Value.Values.Sum();

    if (sum > 0)
    {
        var normalizedValues = vector.Value.ToDictionary(kv => kv.Key, kv => kv.Value / sum);
        vectors[vector.Key] = normalizedValues;
    }
}

JsonSerializerOptions options = new()
{ 
    WriteIndented = true 
};

string json = JsonSerializer.Serialize(vectors, options);
File.WriteAllText("Resources/Vectors.json", json);

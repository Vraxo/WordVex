using System.Text.Json;

public class WordVectorizer
{
    public List<string> UniqueWords = [];
    public Dictionary<string, Dictionary<string, float>> Vectors = [];

    public void ProcessText(string filePath)
    {
        string text = File.ReadAllText(filePath).ToLower();
        string[] words = text.Split(' ');

        BuildUniqueWordsAndVectors(words);
        InitializeVectorRelationships();
        UpdateWordRelationships(words);
        NormalizeVectors();
    }

    private void BuildUniqueWordsAndVectors(string[] words)
    {
        foreach (string word in words)
        {
            if (!UniqueWords.Contains(word))
            {
                UniqueWords.Add(word);
                Vectors.Add(word, new Dictionary<string, float>());
            }
        }
    }

    private void InitializeVectorRelationships()
    {
        foreach (var vector in Vectors)
        {
            foreach (string uniqueWord in UniqueWords)
            {
                if (vector.Key != uniqueWord)
                {
                    vector.Value.Add(uniqueWord, 0);
                }
            }
        }
    }

    private void UpdateWordRelationships(string[] words)
    {
        for (int i = 0; i < words.Length; i++)
        {
            string currentWord = words[i];

            UpdatePreviousWords(words, currentWord, i);
            UpdateNextWords(words, currentWord, i);
        }
    }

    private void UpdatePreviousWords(string[] words, string currentWord, int currentIndex)
    {
        if (currentIndex > 0)
        {
            for (int j = 1; j <= currentIndex; j++)
            {
                string previousWord = words[currentIndex - j];
                if (currentWord != previousWord)
                {
                    Vectors[currentWord][previousWord] += 1F / j;
                }
            }
        }
    }

    private void UpdateNextWords(string[] words, string currentWord, int currentIndex)
    {
        if (currentIndex < words.Length - 1)
        {
            for (int j = 1; j <= words.Length - currentIndex - 1; j++)
            {
                string nextWord = words[currentIndex + j];
                if (currentWord != nextWord)
                {
                    Vectors[currentWord][nextWord] += 1F / j;
                }
            }
        }
    }

    private void NormalizeVectors()
    {
        foreach (var vector in Vectors)
        {
            float sum = vector.Value.Values.Sum();

            if (sum > 0)
            {
                var normalizedValues = vector.Value.ToDictionary(kv => kv.Key, kv => kv.Value / sum);
                Vectors[vector.Key] = normalizedValues;
            }
        }
    }

    public void SaveVectorsToJson(string outputPath)
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(Vectors, options);
        File.WriteAllText(outputPath, json);
    }

    public void LoadVectorsFromJson(string filePath)
    {
         string json = File.ReadAllText(filePath);
         Vectors = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, float>>>(json);
         UniqueWords = Vectors.Keys.ToList();
    }

    public float CalculateSimilarity(string word1, string word2)
    {
        if (!Vectors.ContainsKey(word1) || !Vectors.ContainsKey(word2))
        {
            return 0;
        }

        var vector1 = Vectors[word1];
        var vector2 = Vectors[word2];

        float dotProduct = 0;
        foreach (var word in UniqueWords)
        {
            dotProduct += vector1.GetValueOrDefault(word, 0) * vector2.GetValueOrDefault(word, 0);
        }

        // Calculate magnitudes
        float magnitude1 = (float)Math.Sqrt(vector1.Values.Sum(v => v * v));
        float magnitude2 = (float)Math.Sqrt(vector2.Values.Sum(v => v * v));

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0;
        }

        return dotProduct / (magnitude1 * magnitude2);
    }

    public List<(string word, float similarity)> GetMostSimilarWords(string targetWord, int n)
    {
        List<(string word, float similarity)> similarities = new();

        if (!Vectors.ContainsKey(targetWord))
        {
            return similarities;
        }

        foreach (string word in UniqueWords)
        {
            if (word != targetWord)
            {
                float similarity = CalculateSimilarity(targetWord, word);
                similarities.Add((word, similarity));
            }
        }

        return similarities
            .OrderByDescending(s => s.similarity)
            .Take(n)
            .ToList();
    }
}

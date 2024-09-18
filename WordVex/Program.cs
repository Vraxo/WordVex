public class Program
{
    public static void Main()
    {
        // Create an instance of WordVectorizer
        WordVectorizer vectorizer = new();

        // File paths
        string inputFilePath = "Resources/Text.txt";
        string outputFilePath = "Resources/Vectors.json";

        // Process the text file to build and normalize vectors
        vectorizer.ProcessText(inputFilePath);

        // Save the vectors to a JSON file
        vectorizer.SaveVectorsToJson(outputFilePath);

        // Load the vectors from the JSON file
        vectorizer.LoadVectorsFromJson(outputFilePath);

        // Calculate and print similarity between two words
        string word1 = "taylor";  // Replace with actual words present in your text
        string word2 = "swift";   // Replace with actual words present in your text
        float similarity = vectorizer.CalculateSimilarity(word1, word2);
        Console.WriteLine($"Similarity between '{word1}' and '{word2}': {similarity}");

        // Find and print the top 5 most similar words to a given word
        int topN = 5;
        string targetWord = "singer"; // Replace with actual word present in your text
        var mostSimilarWords = vectorizer.GetMostSimilarWords(targetWord, topN);
        Console.WriteLine($"\nTop {topN} words most similar to '{targetWord}':");
        foreach (var (word, sim) in mostSimilarWords)
        {
            Console.WriteLine($"{word}: {sim}");
        }

        Console.ReadLine();
    }
}
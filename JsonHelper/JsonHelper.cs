using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonHelper
{
    /// <summary>
    /// Provides helper methods for serializing and deserializing objects to and from JSON files.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// The default JSON serializer options used for all operations.
        /// </summary>
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Serializes the specified object and saves it as a JSON file at the given file path.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="filePath">The path to the file where the JSON will be saved.</param>
        /// <param name="data">The object to serialize and save.</param>
        /// <exception cref="Exception">Throws if serialization or file writing fails.</exception>
        public static void SaveToFile<T>(string filePath, T data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, _options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON to file: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads and deserializes an object of type <typeparamref name="T"/> from a JSON file at the given file path.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="filePath">The path to the JSON file to load.</param>
        /// <returns>
        /// The deserialized object of type <typeparamref name="T"/>, or the default value of <typeparamref name="T"/> if loading fails.
        /// </returns>
        public static T LoadFromFile<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("JSON file not found", filePath);

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json, _options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON from file: {ex.Message}");
                return default;
            }
        }
    }
}

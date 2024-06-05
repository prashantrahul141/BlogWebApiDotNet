using System.Collections;

namespace BlogWebApiDotNet.Utils
{
    /// <summary>
    ///  Class <c>LoadDotEnv</c> - Utiliy class to load and validate environment variables.
    /// </summary>
    public class LoadDotEnv
    {
        /// Public flag to store verification state.
        public bool isValidated = false;

        /// keys which were verified and are GURANTED to be the environment.

        Dictionary<string, string> validatedKeys = [];

        /// <summary>
        /// Constructor <c>LoadDotEnv</c> creates new instance.
        /// </summary>
        public LoadDotEnv() { }

        /// <summary>
        /// Constructor <c>LoadDotEnv</c> loads env from a file.
        /// </summary>
        /// <param name="m_filePath">filepath of env file.</param>
        public LoadDotEnv(string m_filePath)
        {
            if (!File.Exists(m_filePath))
            {
                Console.WriteLine(".env file doesn't exist.");
                return;
            }

            // for each line in the file
            foreach (string line in File.ReadAllLines(m_filePath))
            {
                // split it
                string[] lineParts = line.Split("=");
                if (lineParts.Length != 2)
                {
                    throw new Exception("The .env file provided is in incorrect format.");
                }

                // set env var.
                Environment.SetEnvironmentVariable(lineParts[0], lineParts[1]);
            }
        }

        /// <summary>
        /// Method <c>ValidateEnv</c> validates current set environment variables.
        /// </summary>
        /// <param name="m_KeysToValidate">Array of string keys to verify.</param>
        public void ValidateEnv(ref string[] m_KeysToValidate)
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            foreach (string key in m_KeysToValidate)
            {
                var value = environmentVariables[key] as string;
                if (value == null)
                {
                    Console.WriteLine($"Failed to validate environment variable {key} is not set.");
                    continue;
                }

                validatedKeys.Add(key, value);
            }

            isValidated = true;
        }

        /// <summary>
        /// Method <c>GetKeyOrThrow</c> returns a key, throws error if it doesnt exist.
        /// </summary>
        /// <param name="key">key to find</param>
        public string GetKeyOrThrow(string key)
        {
            if (!isValidated)
            {
                throw new Exception(
                    "Env vars has not been validated yet. try running EnvLoader.ValidateEnv() before retrieving it."
                );
            }

            if (!validatedKeys.ContainsKey(key))
            {
                throw new Exception($"Env var {key} is not present in LoadDotEnv.");
            }

            return validatedKeys[key];
        }

        /// <summary>
        /// Method <c>UnsafeGet</c> returns a key, doesn't throw error if it doesnt exist.
        /// </summary>
        /// <param name="key">key to find</param>
        public string UnsafeGetKey(string key)
        {
            if (!isValidated)
            {
                Console.WriteLine("Env vars has not been validated yet.");
            }

            if (!validatedKeys.ContainsKey(key))
            {
                Console.WriteLine($"Env var {key} is not present in LoadDotEnv, Returning null");
            }

            return validatedKeys[key];
        }
    }
}

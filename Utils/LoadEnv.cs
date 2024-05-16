using System.Collections;

namespace BlogWebApiDotNet.Utils
{
    public class LoadDotEnv
    {
        public bool isValidated = false;
        public string[] validatedKeys = [];

        public LoadDotEnv(string m_filePath)
        {
            if (!File.Exists(m_filePath))
            {
                Console.WriteLine(".env file doesn't exist.");
                return;
            }

            foreach (string line in File.ReadAllLines(m_filePath))
            {
                string[] lineParts = line.Split("=");
                if (lineParts.Length != 2)
                {
                    throw new Exception("The .env file provided is in incorrect format.");
                }

                Environment.SetEnvironmentVariable(lineParts[0], lineParts[1]);
            }
        }

        public void ValidateEnv(ref string[] m_KeysToValidate)
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            foreach (string key in m_KeysToValidate)
            {
                if (environmentVariables[key] == null)
                {
                    Console.WriteLine($"Failed to validate environment variable {key} is not set.");
                }
            }

            isValidated = true;
            validatedKeys = m_KeysToValidate;
        }

        public string GetKeyOrThrow(string key)
        {
            if (!isValidated)
            {
                Console.WriteLine(
                    "Env vars has not been validated yet. try running EnvLoader.ValidateEnv() before retrieving it."
                );
            }

            if (!validatedKeys.Contains(key))
            {
                Console.WriteLine($"Env var {key} is not validated by LoadDotEnv.");
            }

            return Environment.GetEnvironmentVariable(key)!;
        }
    }
}

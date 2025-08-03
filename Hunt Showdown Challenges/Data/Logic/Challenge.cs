using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hunt_Showdown_Challenges.Data.Logic
{
    public class Challenge : Models.Challenge
    {
        static string dataFile = "Data/HuntChallenges.json";
        static string dataStorageDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Hunt Showdown Challenges";
        static string challengeFile = $"{dataStorageDirectory}\\Active Challenges.txt";
        public static IEnumerable<Challenge>? Challenges { get; set; }

        /// <summary>
        /// Load challenges from the data file. Throws FileNotFoundException on empty file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public async static Task<IEnumerable<Challenge>> LoadChallengesAsync()
        {
            if (!System.IO.File.Exists(dataFile)) { throw new System.IO.FileNotFoundException(); }

            var strChallenges = await System.IO.File.ReadAllTextAsync(dataFile, CancellationToken.None);
            Challenges = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Challenge>>(strChallenges);
            if (Challenges == null) { throw new System.IO.FileNotFoundException(); }

            //STORE THE ORIGINAL DATA
            foreach (var challenge in Challenges)
            {
                challenge.OriginalDescription = challenge.Description;
                challenge.OriginalTitle = challenge.Title;
            }
            //STORE THE ORIGINAL DATA

            return Challenges;
        }

        /// <summary>
        /// Update the Challenge File with the new challenge
        /// </summary>
        /// <param name="challenge">The new challenge to append to the file</param>
        /// <param name="includeDescription">Should the file include the Description</param>
        /// <returns></returns>
        public async static Task UpdateChallengeFile(Challenge challenge, bool includeDescription = false)
        {
            if (!System.IO.Directory.Exists(dataStorageDirectory)) { System.IO.Directory.CreateDirectory(dataStorageDirectory); }
            if (!System.IO.File.Exists(challengeFile)) { System.IO.File.Create(challengeFile); }
            await System.IO.File.AppendAllTextAsync(challengeFile, $"{challenge.Title}{Environment.NewLine}");
            if (includeDescription)
            {
                await System.IO.File.AppendAllTextAsync(challengeFile, $"{challenge.Description}{Environment.NewLine}{Environment.NewLine}");
            }
        }

        /// <summary>
        /// Erase the content of the Challenge File0 
        /// </summary>
        /// <returns></returns>
        public async static Task ResetChallengeFile()
        {
            if (!System.IO.Directory.Exists(dataStorageDirectory)) { System.IO.Directory.CreateDirectory(dataStorageDirectory); }
            if (!System.IO.File.Exists(challengeFile)) { System.IO.File.Create(challengeFile); }
            try
            {
                await System.IO.File.WriteAllTextAsync(challengeFile, string.Empty);
            }
            catch { }
        }
    }
}

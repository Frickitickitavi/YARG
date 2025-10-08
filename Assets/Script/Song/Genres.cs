using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YARG.Core.Logging;
using YARG.Helpers;

namespace YARG.Song
{
    public static partial class Genres
    {
        [Serializable]
        // This is a serialized class; naming conventions are JSON's, not C#'s
        [SuppressMessage("ReSharper", "All")]
        public class SubgenreMapping
        {
            public string genre;
            public Dictionary<string, string>? localizations;
        }

        private const string GENRE_COMMIT_URL =
            "https://api.github.com/repos/YARC-Official/Genrelizer/commits?per_page=1";

        private const string GENRE_ZIP_URL =
            "https://github.com/YARC-Official/Genrelizer/archive/refs/heads/master.zip";

        public const string GENRE_REPO_FOLDER = "Genrelizer-master";

        private const string GENRE_ALIASES_FILENAME = "genreAliases.json";
        private const string SUBGENRE_ALIASES_FILENAME = "subgenreAliases.json";
        private const string SUBGENRE_MAPPINGS_FILENAME = "subgenreMappings.json";

#if UNITY_EDITOR
        // The editor does not track the contents of folders that end in ~,
        // so use this to prevent Unity from stalling due to importing freshly-downloaded sources
        public static readonly string GenresFolder = Path.Combine(PathHelper.StreamingAssetsPath, "genres~");
#else
        public static readonly string GenresFolder = Path.Combine(PathHelper.StreamingAssetsPath, "genres");
#endif

        private static Dictionary<string, string> _genreAliases = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, string> _subgenreAliases = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, SubgenreMapping> _subgenreMappings = new(StringComparer.OrdinalIgnoreCase);

        public static async UniTask LoadGenreMappings(LoadingContext context)
        {
            if (!GlobalVariables.OfflineMode)
            {
                await DownloadGenreMappings(context);
            }

            context.SetSubText("Loading genre mappings...");
            ReadGenreAliases();
            ReadSubgenreAliases();
            ReadSubgenreMappings();
        }

        private static async UniTask DownloadGenreMappings(LoadingContext context)
        {
            context.SetLoadingText("Downloading genre mappings...");

            // Create the sources folder if it doesn't exist
            Directory.CreateDirectory(GenresFolder);

            context.SetSubText("Checking version...");
            string genreVersionPath = Path.Combine(GenresFolder, "version.txt");
            string currentVersion = null;
            try
            {
                if (File.Exists(genreVersionPath))
                    currentVersion = await File.ReadAllTextAsync(genreVersionPath);
            }
            catch (Exception e)
            {
                YargLogger.LogException(e, "Failed to get current song genre version.");
            }

            // Look for new version
            context.SetSubText("Looking for new version...");
            string newestVersion = null;
            try
            {
                // Retrieve sources file
                var request = (HttpWebRequest) WebRequest.Create(GENRE_COMMIT_URL);
                request.UserAgent = "YARG";
                request.Timeout = 2500;

                // Send the request and wait for the response
                using var response = await request.GetResponseAsync();
                using var reader = new StreamReader(response.GetResponseStream()!, Encoding.UTF8);

                // Read the JSON
                var json = JArray.Parse(await reader.ReadToEndAsync());
                newestVersion = json[0]["sha"]!.ToString();
            }
            catch (Exception e)
            {
                YargLogger.LogException(e, "Failed to get newest song genre version. Skipping.");
            }

            // If we failed to find the newest version, finish
            if (newestVersion == null)
            {
                return;
            }

            // If up to date, finish
            var repoDir = Path.Combine(GenresFolder, GENRE_REPO_FOLDER);
            if (newestVersion == currentVersion && Directory.Exists(repoDir))
            {
                return;
            }

            // Otherwise, update!
            try
            {
                // Download
                context.SetSubText("Downloading new version...");
                string zipPath = Path.Combine(GenresFolder, "update.zip");
                using (var client = new WebClient())
                {
                    await UniTask.RunOnThreadPool(() => { client.DownloadFile(GENRE_ZIP_URL, zipPath); });
                }

                // Delete the old folder
                if (Directory.Exists(repoDir))
                {
                    Directory.Delete(repoDir, true);
                }

                // Extract the base and extras folder
                context.SetSubText("Extracting new version...");
                ZipFile.ExtractToDirectory(zipPath, GenresFolder);

                // Delete the random folders
                var ignoreFolder = Path.Combine(repoDir, "ignore");
                if (Directory.Exists(ignoreFolder))
                {
                    Directory.Delete(ignoreFolder, true);
                }

                var githubFolder = Path.Combine(repoDir, ".github");
                if (Directory.Exists(githubFolder))
                {
                    Directory.Delete(githubFolder, true);
                }

                // Delete the random files
                foreach (var file in Directory.EnumerateFiles(repoDir))
                {
                    if (!file.EndsWith(".json"))
                    {
                        File.Delete(file);
                    }
                }

                // Create the version txt
                await File.WriteAllTextAsync(Path.Combine(GenresFolder, "version.txt"), newestVersion);

                // Delete the zip
                File.Delete(zipPath);
            }
            catch (Exception e)
            {
                YargLogger.LogException(e, "Failed to download newest song genre version.");
            }
        }

        private static void ReadGenreAliases()
        {
            var path = Path.Combine(GenresFolder, GENRE_REPO_FOLDER, GENRE_ALIASES_FILENAME);
            var mappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
            foreach (var key in mappings.Keys)
            {
                _genreAliases.Add(key, mappings[key]);
            }
        }

        private static void ReadSubgenreAliases()
        {
            var path = Path.Combine(GenresFolder, GENRE_REPO_FOLDER, SUBGENRE_ALIASES_FILENAME);
            var mappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
            foreach (var key in mappings.Keys)
            {
                _subgenreAliases.Add(key, mappings[key]);
            }
        }

        private static void ReadSubgenreMappings()
        {
            var path = Path.Combine(GenresFolder, GENRE_REPO_FOLDER, SUBGENRE_MAPPINGS_FILENAME);
            var mappings = JsonConvert.DeserializeObject<Dictionary<string, SubgenreMapping>>(File.ReadAllText(path));
            foreach (var key in mappings.Keys)
            {
                _subgenreMappings.Add(key, mappings[key]);
            }
        }

        public static (string? genre, string? subgenre) GetGenresOrDefault(string? rawGenre, string? rawSubgenre)
        {
            if (string.IsNullOrEmpty(rawGenre))
            {
                // If neither value is provided, return nothing
                if (string.IsNullOrEmpty(rawSubgenre))
                {
                    return (null, null);
                }

                // If only a subgenre is provided (not expected), treat it as the genre
                return HandleLoneGenre(rawSubgenre);
            }

            if (string.IsNullOrEmpty(rawSubgenre))
            {
                return HandleLoneGenre(rawGenre);
            }

            return HandleGenreSubgenrePair(rawGenre, rawSubgenre);
        }

        private static (string genre, string? subgenre) HandleLoneGenre(string rawGenre) {
            // Apply any genre alias, if present
            var processedGenre = _genreAliases.GetValueOrDefault(rawGenre, rawGenre);

            if (GENRE_LOCALIZATION_KEYS.ContainsKey(processedGenre))
            {
                // We have an official genre, so localize it and return it with no subgenre
                return (Localization.Localize.KeyFormat("Menu.MusicLibrary.Genre", GENRE_LOCALIZATION_KEYS[processedGenre]), null);
            }

            // This isn't an official genre, so we're going to use it as a subgenre

            // First, apply a subgenre alias, if present
            var subgenre = _subgenreAliases.GetValueOrDefault(rawGenre, rawGenre);

            // Is this a known subgenre?
            if (_subgenreMappings.ContainsKey(subgenre))
            {
                var mapping = _subgenreMappings[subgenre];

                // Localize the subgenre if applicable
                if (mapping.localizations is not null && mapping.localizations.ContainsKey(Localization.LocalizationManager.CultureCode))
                {
                    subgenre = mapping.localizations[Localization.LocalizationManager.CultureCode];
                }
                

                return (
                    genre: Localization.Localize.KeyFormat("Menu.MusicLibrary.Genre", GENRE_LOCALIZATION_KEYS[mapping.genre]),
                    subgenre
                );
            }

            // Not a known subgenre, so categorize as "other"
            return (Localization.Localize.Key("Menu.MusicLibrary.Genre.Other"), subgenre);
        }

        private static (string genre, string? subgenre) HandleGenreSubgenrePair(string rawGenre, string rawSubgenre)
        {
            string genre = null;
            string subgenre = null;

            // Check if this is a telltale value pair from Magma, for which we have a ready-to-go mapping
            if (MAGMA_MAPPINGS.ContainsKey((rawGenre, rawSubgenre)))
            {
                var magmaMapping = MAGMA_MAPPINGS[(rawGenre, rawSubgenre)];

                // Localize the returned genre directly
                genre = Localization.Localize.KeyFormat("Menu.MusicLibrary.Genre", GENRE_LOCALIZATION_KEYS[magmaMapping.genre]);

                // Subgenre localizations come from Genrelizer
                if (magmaMapping.subgenre is not null)
                {
                    var magmaSubgenreMapping = _subgenreMappings[magmaMapping.subgenre];
                    if (magmaSubgenreMapping?.localizations is not null)
                    {
                        subgenre = magmaSubgenreMapping.localizations.GetValueOrDefault(Localization.LocalizationManager.CultureCode, magmaMapping.subgenre);
                    }
                    else
                    {
                        subgenre = magmaMapping.subgenre;
                    }
                }
                else
                {
                    subgenre = magmaMapping.subgenre;
                }

                return (genre, subgenre);
            }

            // Apply a genre alias, if present
            var aliasedGenre = _genreAliases.GetValueOrDefault(rawGenre, rawGenre);

            // Also check if the subgenre is the same as the genre (accounting for genre aliases). If so, discard the subgenre
            // and treat this as a lone genre
            var subgenreAsGenre = _genreAliases.GetValueOrDefault(rawSubgenre, null);
            if (subgenreAsGenre == aliasedGenre)
            {
                return HandleLoneGenre(aliasedGenre);
            }


            if (GENRE_LOCALIZATION_KEYS.ContainsKey(aliasedGenre))
            {
                // The genre is an official one! Localize it, and then we can move on to the subgenre
                genre = Localization.Localize.KeyFormat("Menu.MusicLibrary.Genre", GENRE_LOCALIZATION_KEYS[aliasedGenre]);
            }

            // We have a nonstandard genre AND a subgenre. Let's see if we can salvage an official genre from the nonstandard
            // one by treating it as a subgenre for a moment
            var genreAsSubgenre = _subgenreAliases.GetValueOrDefault(rawGenre, rawGenre);

            if (_subgenreMappings.ContainsKey(genreAsSubgenre))
            {
                // Treating the nonstandard genre as a subgenre led us to a mapping! We can use the genre from this mapping
                // as the final genre. The original nonstandard genre gets discarded, because we'd rather honor the charter's
                // intended subgenre
                var surrogateGenre = _subgenreMappings[genreAsSubgenre].genre;
                genre = Localization.Localize.KeyFormat("Menu.MusicLibrary.Genre", GENRE_LOCALIZATION_KEYS[surrogateGenre]);
            }

            // Time to sort out the subgenre. Start by applying an alias, if present
            var aliasedSubgenre = _subgenreAliases.GetValueOrDefault(rawSubgenre, rawSubgenre);

            // See if we have a mapping for this subgenre
            if (_subgenreMappings.ContainsKey(aliasedSubgenre))
            {
                var mapping = _subgenreMappings[aliasedSubgenre];

                // If the raw genre never yielded an official genre, we can defer to the subgenre mapping
                if (genre is null)
                {
                    var genreFromSubgenre = GENRE_LOCALIZATION_KEYS[mapping.genre];
                    genre = Localization.Localize.KeyFormat("Menu.MusicLibrary.Genre", GENRE_LOCALIZATION_KEYS[genreFromSubgenre]);
                }

                if (mapping.localizations is not null)
                {
                    subgenre = mapping.localizations.GetValueOrDefault(Localization.LocalizationManager.CultureCode, aliasedSubgenre);
                } else
                {
                    subgenre = aliasedSubgenre;
                }
            }

            else
            {
                // The subgenre wasn't recognized, so we'll just use it as-is
                subgenre = aliasedSubgenre;

                // If we never figured out a genre, default to Other
                genre ??= Localization.Localize.Key("Menu.MusicLibrary.Genre.Other");
            }

            return (genre, subgenre);
        }
    }
}

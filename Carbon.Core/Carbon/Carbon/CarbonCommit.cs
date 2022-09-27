using Newtonsoft.Json;

namespace Carbon.Core
{
    public class CarbonCommit
    {
        public static string Source { get; private set; }
        public static Commit Current { get; private set; }

        public static void Refresh ()
        {
            Source = Properties.Resources.git;
            Current = JsonConvert.DeserializeObject<Commit> ( Properties.Resources.git );
        }
    }

    public class Commit
    {
        [JsonProperty("shortSHA")]
        public string ShortSHA { get; set; }

        [JsonProperty ( "longSHA" )]
        public string LongSHA { get; set; }

        [JsonProperty ( "changeset" )]
        public int Changeset { get; set; }

        [JsonProperty ( "author_name" )]
        public string AuthorName { get; set; }

        [JsonProperty ( "author_email" )]
        public string AuthorEmail { get; set; }

        [JsonProperty ( "date" )]
        public string Date { get; set; }

        [JsonProperty ( "branch" )]
        public string Branch { get; set; }

        [JsonProperty ( "message" )]
        public string Message { get; set; }

        [JsonProperty ( "repository_name" )]
        public string Repository { get; set; }

        public class Change
        {
            [JsonProperty ( "path" )]
            public string Path { get; set; }

            [JsonProperty ( "type" )]
            public ChangeType Type { get; set; }

            public enum ChangeType
            {
                Added,
                Modified,
                Deleted
            }
        }
    }
}

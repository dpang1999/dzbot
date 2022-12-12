using Discord;
using Discord.Commands;
using DiscordBot.Utilities;
using DiscordBot.Utilities.Managers.Storage;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class GeneralModules : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Alias("echo")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
            => ReplyAsync(echo);
        

        [Command("save")]
        [Summary("Saves the current bot data, only the bot admin can issue it")]
        public async Task Save()
        {
            if (Context.Message.Author.Id != 107666976062574592) return;

            //Use this to get your user id.
            Console.WriteLine(Context.Message.Author.Id);
            await ReplyAsync("Saving...");
            DataStorageManager.Current.SaveData();
        }

        [Command("load")]
        [Summary("Loads the bot data from its save, only the bot admin can issue it")]
        public async Task Load()
        {
            if (Context.Message.Author.Id != 107666976062574592) return;

            await ReplyAsync("Loading...");
            DataStorageManager.Current.LoadData();
        }

        [Command("change prefix")]
        [Alias("cp")]
        [Summary("Changes the bot command prefix for this server")]
        public Task ChangePrefix([Summary("New Prefix")] char prefix)
        {
            var id = Context.Guild.Id;
            DataStorageManager.Current[id].CommandPrefix = prefix;
            AutoSaveManager.ReduceIntervalByChangePriority(ChangePriority.GuildDataChange);
            return ReplyAsync($"The command prefix for this server is now {DataStorageManager.Current[id].CommandPrefix}");
        }

       //The KQM Guide one
        private List<String> kqmGuides = new List<String>()
        {
            "albedo", "aloy", "amber", "anemo traveler", "ayaka", "ayato", "barbara", "beidou", "bennett", "chongyun",
            "diluc", "diona", "electro traveler", "eula", "ganyu", "geo traveler", "gorou", "heizou", "hu tao", "itto",
            "jean", "kaeya", "kazuha", "keqing", "klee", "kokomi", "lisa", "mona", "ningguang", "noelle", "qiqi",
            "raiden", "razor", "rosaria", "sara", "shenhe", "sucrose", "tartaglia", "thoma", "tighnari", "venti",
            "xiangling", "xiao", "xingqiu", "xinyan", "yae", "yanfei", "yelan", "yoimiya", "yun jin", "zhongli",
            "collei"

        };
        private List<String> kqmQuickGuides = new List<String>()
        {
            "cyno", "fischl", "layla", "nahida", "nilou",
            "wanderer", "faruzan", "candace", "layla"
        };
        [Command("kqm")]
        [Summary("Posts relevant keqingmains.com guides")]
        public Task kqm([Summary("The character to lookup")]string character)
        {
            character = character.ToLower();
            switch (character)
            {
                case "childe":
                    character = "tartaglia";
                    break;
                case "yae miko":
                case "miko":
                    character = "yae";
                    break;
                case "kujou sara":
                    character = "sara";
                    break;
                case "xq":
                    character = "xingqiu";
                    break;
                case "xl":
                    character = "xiangling";
                    break;
                case "raiden shogun":
                case "shogun":
                    character = "raiden";
                    break;
                case "yun jin":
                    character = "yunjin";
                    break;
                case "hu tao":
                    character = "hu-tao";
                    break;
                case "electro traveler":
                    character = "electro-traveler";
                    break;
                case "anemo traveler":
                    character = "anemo-traveler";
                    break;
                case "geo traveler":
                    character = "geo-traveler";
                    break;
            }
            
            if(kqmGuides.Contains(character))
                return ReplyAsync($"https://keqingmains.com/{character}");
            else if (kqmQuickGuides.Contains(character))
                return ReplyAsync($"https://keqingmains.com/q/{character}-quickguide/");
            else
                return ReplyAsync($"Guide for {character} not found.");
        }

        private Dictionary<string, List<string>> allQuotes = new Dictionary<string, List<string>>();
        public void readInQuotes()
        {
            List<string> quotes = new List<string>();
            string id = Context.Guild.Id.ToString();
            string filename = $"quotes\\{id}.txt";
            using (StreamReader sr = File.OpenText(filename))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    quotes.Add(line);
                }
            }
            allQuotes.Add(id,quotes);
        }
        
        [Command("quoteAdd")]
        [Summary("Add a quote")]
        public Task quoteAdd([Remainder] string quote)
        {
            string id = Context.Guild.Id.ToString();
            string filename = $"quotes\\{id}.txt";
            string url = null;
            if (!allQuotes.ContainsKey(id))
            {
                readInQuotes();
            }
            if(Context.Message.Attachments.Count > 0)
            {
                url = Context.Message.Attachments.First().Url;
            }
            using (StreamWriter sw = File.AppendText(filename))
            {
                sw.WriteLine(quote + " " + url);
            }
            List<string> quotes = allQuotes[id];
            quotes.Add(quote + " " + url);
            return ReplyAsync("Added: " + quote + " " + url);
        }

        
        [Command("quote")]
        [Summary("Pulls up a quote")]
        public Task quote(int num)
        {
            string id = Context.Guild.Id.ToString();
            if (!allQuotes.ContainsKey(Context.Guild.Id.ToString()))
            {
                readInQuotes();
            }
            List<string> quotes = allQuotes[id];
            if (num <= quotes.Count)
            {
                return ReplyAsync(quotes[num-1]);
            }
            else
            {
                return ReplyAsync("Out of range, max: " + quotes.Count + " for this server.");
            }
        }

        [Command("randQuote")]
        [Summary("Pulls up a random quote")]
        public Task randQuote()
        {
            List<string> quotes = new List<string>();
            string id = Context.Guild.Id.ToString();
            if (!allQuotes.ContainsKey(Context.Guild.Id.ToString()))
            {
                string filename = $"quotes\\{id}.txt";
                using (StreamReader sr = File.OpenText(filename))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        quotes.Add(line);
                    }
                }
                allQuotes.Add(id,quotes);
            }
            quotes = allQuotes[id];
            int num = rand.Next(quotes.Count + 1);
            return ReplyAsync(quotes[num]);
        }

        // Stupid Dice Rolling Commands of ZERO interest
        Random rand = new Random();
        [Command("d20")]
        [Summary("Roll a d20 lol")]
        public Task d20()
        {
            return ReplyAsync(""+(rand.Next(20) + 1));
        }

        private List<String> dps = new List<String>()
        {
            "Ashe", "Bastion", "Cassidy", "Echo", "Genji", "Hanzo", "Junkrat", "Mei", "Pharah",
            "Reaper", "Soujourn", "Soldier: 76", "Sombra", "Symmetra", "Torbjorn", "Tracer", "Widowmaker"
        };
        [Command("randDPS")]
        [Summary("Roll a random tank hero from Overwatch 2")]
        public Task randDPS()
        {
            return ReplyAsync(""+dps[rand.Next(dps.Count)]);
        }

        private List<String> support = new List<String>()
        {
            "Ana", "Baptiste", "Brigitte", "Kiriko", "Lucio", "Mercy", "Moira", "Zenyatta"
        };
        [Command("randSupport")]
        [Summary("Roll a random support hero from Overwatch 2")]
        public Task randSupport()
        {
            return ReplyAsync(""+support[rand.Next(support.Count)]);
        }

        private List<String> tank = new List<String>()
        {
            "D.Va", "Doomfist", "Junker Queen", "Orisa", "Reinhardt", "Roadhog",
            "Sigma", "Winston", "Wrecking Ball", "Zarya"
        };
        [Command("randTank")]
        [Summary("Roll a random DPS hero from Overwatch 2")]
        public Task randTank()
        {
            return ReplyAsync(""+tank[rand.Next(tank.Count)]);
        }
        
        
        ///TODO: Add the params to the help info.
        ///TODO: Specific command help
        /// This command automatically generates a help message using the Summary
        /// attribute provided by the framework. It is very fast thanks to c# reflections, 
        /// but if you desire you can use this function to generate the message once, 
        /// and then save it to a static message to be faster. Just remember to run it 
        /// again when you change something.
        [Command("help")]
        [Summary("the help command. I think is very self explanatory.")]
        public async Task HelpCommand([Remainder] string command = "")
        {
            var embed = new EmbedBuilder()
            {
                Title = "This is the list of all commands for this bot",
                Color = new Color(10, 180, 10)
            };

            var modules = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ModuleBase<SocketCommandContext>))).ToList();

            var description = new StringBuilder();
            description.AppendLine();

            modules.ForEach(t =>
            {
                var moduleName = t.Name.Remove(t.Name.IndexOf("Module"));
                description.AppendLine($"**{moduleName} Commands**");
             
                var methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();

                var group = t.GetCustomAttribute<GroupAttribute>();

                methods.ForEach(mi =>
                {
                    var command = mi.GetCustomAttribute<CommandAttribute>();
                    if (command == null) return;

                    var summary = mi.GetCustomAttribute<SummaryAttribute>();
                    var aliases = mi.GetCustomAttribute<AliasAttribute>();
                    var groupName = "";
                    var commandPrefix = DataStorageManager.Current[Context.Guild.Id].CommandPrefix;

                    if (group != null) groupName = group.Prefix + " ";

                    description.Append($"**{commandPrefix}{groupName}{command.Text}**");

                    if (aliases != null)
                        Array.ForEach(aliases.Aliases, a => description.Append($" or **{commandPrefix}{groupName}{(a == "**" ? "\\*\\*" : a)}**"));

                    if (summary != null)
                        description.Append($"\n{summary.Text}");

                    description.AppendLine("\n");
                });
            });

            embed.Description = description.ToString();

            await ReplyAsync(embed: embed.Build());
        }

        private Color GetColorFromSting(string str)
        {
            int dividerIndex = (int)Math.Floor(str.Length / 3d);

            int r = Math.Abs(str.Substring(0, dividerIndex).GetHashCode() % 255);
            int g = Math.Abs(str.Substring(dividerIndex, 2 * dividerIndex).GetHashCode() % 255);
            int b = Math.Abs(str.Remove(0, 2 * dividerIndex).GetHashCode() % 255);

            return new Color(r, g, b);
        }
    }
}
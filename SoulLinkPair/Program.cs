using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Soullink {
    public class ObjPair<X, Y> {
        public X Item1 { get; set; }
        public Y Item2 { get; set; }

        public void SetX(X _item1) {
            Item1 = _item1;
        }

        public void SetY(Y _item2) {
            Item2 = _item2;
        }
    }

    public class Pair {
        public Pokemon P1 { get; set; } = new Pokemon();
        public Pokemon P2 { get; set; } = new Pokemon();
        public int Score { get; set; } = 0;

        public bool HasTypes(string t1, string t2) {
            return P1.Type == t1
                || P1.Type == t2
                || P2.Type == t1
                || P2.Type == t2;
        }

        public override bool Equals(object? obj) {
            if (obj == null) return false;
            if (!(obj is Pair)) return false;
            Pair other = (Pair)obj;
            return P1.Equals(other.P1)
                || P1.Equals(other.P2)
                || P2.Equals(other.P2)
                || P2.Equals(other.P1);
        }

        public override string ToString() {
            return $"{P1}, {P2}";
        }
    }

    public class Pokemon {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public override bool Equals(object? obj) {
            // Preliminary checks
            if (obj == null || this == null) return false;      // One object is null
            if (obj.GetType() != this.GetType()) return false;  // They're not both Pokemon

            // Convert and post types
            Pokemon that = (Pokemon)obj;
            return this.Type.Equals(that.Type);
        }

        public override int GetHashCode() {
            throw new NotImplementedException();
        }

        public override string ToString() {
            return $"({Name},{Type})";
        }
    }

    public static class Utility {
        public static List<Pair> CopyPairs(List<Pair> pairs) {
            return new List<Pair>(pairs);
        }
    }

    public class Tree {

        public Pair Node { get; set; }
        public int MaxDepth { get; set; }
        public int Score { get; set; }
        public List<Tree> children = new List<Tree>();

        // Generate a team based on selected pokemon
        public Tree(List<Pair> basePairs, List<Pair> allPairs, int depth = 0, int score = 0) {

            if (MaxDepth == 6) { return; }
            
            if (basePairs.Count > 1) {
                MaxDepth = depth;
                Node = basePairs[0];
                Score = score + Node.Score;

                basePairs.RemoveAt(0);
                children.Add(new Tree(basePairs, allPairs, depth + 1, score));
            } else {
                MaxDepth = depth;
                Node = basePairs[0];
                Score = score + Node.Score;

                List<Pair> dupe = Utility.CopyPairs(allPairs);

                dupe = dupe.Where(x => !x.Equals(Node))
                    .ToList();

                if (dupe.Count == 0) { return; }
                foreach (var item in dupe) {
                    children.Add(new Tree(item, dupe, depth + 1, Score));
                }
            }

            children.Sort((x, y) => x.Score.CompareTo(y.Score));

            MaxDepth = children.Select(x => x.MaxDepth).ToList().Max();
        }

        public Tree(Pair p, List<Pair> list, int depth = 0, int score = 0) {
            Node = p;
            MaxDepth = depth;
            Score = score + Node.Score;

            if (MaxDepth == 6) { return; }

            List<Pair> dupe = Utility.CopyPairs(list);
            while (dupe.Contains(p))
                dupe.Remove(p);

            dupe = dupe.Where(x => !x.Equals(Node))
                .ToList();

            if (dupe.Count == 0) { return; }
            foreach (var item in dupe) {
                children.Add(new Tree(item, dupe, depth + 1, Score));
            }

            children.Sort((x, y) => x.Score.CompareTo(y.Score));

            MaxDepth = children.Select(x => x.MaxDepth).ToList().Max();
        }
    }

    public static class Program {
        public static List<Pair> RecursiveTrace(Tree tree) {
            List<Pair> toReturn = new List<Pair>() { tree.Node };
            if (tree.children.Count == 0) { return toReturn; }

            toReturn.AddRange(RecursiveTrace(tree.children.First()));

            return toReturn;
        }

        public static Tree SpecificPokemon(List<Pair> pairs) {
            List<Pair> baseList = new List<Pair>();
            int selection = 0;
            while (selection != -1) {

                // Prompt the user for the Pokemon to add to the base pairs
                Console.WriteLine("Input the number you wish to add:");
                Console.WriteLine("-1: Exit");
                for (int i = 0; i < pairs.Count; i++) {
                    Console.WriteLine($"{i}: {pairs[i]}");
                }

                if (!int.TryParse(Console.ReadLine(), out selection)) { continue; }
                if (selection == -1) { break; }
                if (selection < 0 || selection >= pairs.Count) { continue; }

                baseList.Add(pairs[selection]);
                pairs.RemoveAt(selection);
            }

            pairs = pairs.Where(x => !baseList.Any(y => x.Equals(y))).ToList();

            return new Tree(baseList, pairs, 0, 0);
        }

        public static void Main(string[] args) {
            // Load the base stat information
            List<Pair> pairs = new List<Pair>();
            Dictionary<string, int> baseTotal = new Dictionary<string, int>();
            bool totalCalcPossible = true;
            try {
                using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "StatTotal.csv"))) {
                    reader.ReadLine();
                    while (!reader.EndOfStream) {
                        string[] line = reader.ReadLine().Split(',');
                        baseTotal.Add(line[0], int.Parse(line[1]));
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"{ex.Source}: {ex.Message}. Breaking out.");
                return;
            }

            try {
                using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Soullink.csv"))) {
                    // Find the type of team you want
                    #region Ask who has best team
                    Console.WriteLine("Which of the following you do want to maximize?");
                    Console.WriteLine("1: Player 1 Stats");
                    Console.WriteLine("2: Overall Stats");
                    Console.WriteLine("3: Player 2 Stats");
                    Console.WriteLine("4: Teams With Specific Pokemon");

                    int decision2 = 0;
                    int.TryParse(Console.ReadLine(), out decision2);

                    while (decision2 < 1 || decision2 > 4) {
                        Console.WriteLine("Invalid option. Pick a number from 1 to 4.");
                        int.TryParse(Console.ReadLine(), out decision2);
                    }
                    #endregion

                    Console.WriteLine("Reading file..");

                    #region Generate Pairs
                    // Assume header with format: Location,Pkmn1,Pkmn1Type,Pkmn2,Pkmn2Type,[Dead]
                    string[] line = reader.ReadLine().Split(',');
                    while (!reader.EndOfStream) {
                        line = reader.ReadLine().Split(',');

                        if (string.IsNullOrEmpty(line[1]) || string.IsNullOrEmpty(line[3]) || line[5] == "Dead") { continue; }

                        Pokemon p1 = new Pokemon() { Name = line[1], Type = line[2] };
                        Pokemon p2 = new Pokemon() { Name = line[3], Type = line[4] };
                        pairs.Add(new Pair() {
                            P1 = p1,
                            P2 = p2,
                            Score = decision2 switch {
                                // Scores are based on who you want to have a better team
                                1 => baseTotal[line[1]],
                                3 => baseTotal[line[3]],
                                _ => baseTotal[line[1]] + baseTotal[line[3]] - Math.Abs(baseTotal[line[1]] + baseTotal[line[3]])
                            }
                        });
                    }

                    pairs = pairs
                        .Where(x => x.P1.Name != string.Empty && x.P2.Name != string.Empty)
                        .ToList();
                    #endregion

                    if (decision2 == 4) {
                        SpecificPokemon(pairs);
                        return;
                    }

                    // Generate trees
                    List<Tree> MaxTree = new List<Tree>();
                    pairs.ForEach(x => {
                        Console.WriteLine($"Constructing tree for {x}");
                        MaxTree.Add(new Tree(x, pairs, depth: 1));
                    });

                    MaxTree.Sort((x, y) => {
                        return -x.MaxDepth.CompareTo(y.MaxDepth);
                    });

                    int count = 0;
                    List<ObjPair<int, List<Pair>>> teams = new List<ObjPair<int, List<Pair>>>();
                    MaxTree.ForEach(x => {
                        teams.Add(new ObjPair<int, List<Pair>>() { Item1 = count, Item2 = RecursiveTrace(x) });
                        ++count;
                    });
                    for (int i = 0; i < teams.Count; ++i) {
                        teams[i].SetX(teams[i].Item2.Select(x =>
                            decision2 switch {
                                1 => baseTotal[x.P1.Name],
                                2 => baseTotal[x.P1.Name] + baseTotal[x.P2.Name] - Math.Abs(baseTotal[x.P1.Name] - baseTotal[x.P2.Name]),
                                3 => baseTotal[x.P2.Name],
                            }).Sum());
                    }

                    teams.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                    teams.ForEach(x => {
                        Console.WriteLine(x.Item1);
                        Console.WriteLine(new String('-', 10));
                        x.Item2.ForEach(y => {
                            Console.WriteLine(y);
                        });
                        Console.WriteLine("");
                    });
                }
            } catch (FileNotFoundException e) {
                Console.WriteLine("File was not found. Please put the file in the\nsame directory as the program and rename it to\n\"Soullink.csv\"");
                return;
            }
        }
    }
}
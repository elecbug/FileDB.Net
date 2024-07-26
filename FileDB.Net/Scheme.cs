using FileDB.Net.Utils;

namespace FileDB.Net
{
    public class Scheme
    {
        private string Name__ { get; set; } = "";
        public required string Field { get => Name__; set => Name__ = value.ToRegex(); }
        public required string Description { get; set; }
        public required SchemeType Type { get; set; }

        public static Scheme[] Create(params (string, string, SchemeType)[] values)
        {
            Scheme[] result = new Scheme[values.Length];
            
            for (int i = 0; i < values.Length; i++)
            {
                result[i] = new Scheme()
                {
                    Field = values[i].Item1,
                    Description = values[i].Item2,
                    Type = values[i].Item3,
                };
            }

            return result;
        }

        public static bool HasDuplicated(Scheme[] schemes)
        {
            for (int i = 0; i < schemes.Length - 1; i++)
            {
                for (int j = i + 1; j < schemes.Length; j++)
                {
                    if (schemes[i].Field == schemes[j].Field)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public enum SchemeType
    {
        None = 0,
        IsArray = 1,
        CanString = 2,
        CanInt = 4,
        CanFloat = 8,
        CanBool = 16,
        Nullable = 32,
    }
}
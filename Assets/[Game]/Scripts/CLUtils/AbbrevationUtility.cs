
namespace CLUtils
{
    public static class AbbrevationUtility
    {
        public static string GetAbreviation(long _value)
        {
            if (_value >= 100000000)
            {
                return (_value / 1000000D).ToString("0.#M");
            }
            if (_value >= 1000000)
            {
                return (_value / 1000000D).ToString("0.##M");
            }
            if (_value >= 100000)
            {
                return (_value / 1000D).ToString("0.#k");
            }
            if (_value >= 1000)
            {
                return (_value / 1000D).ToString("0.##k");
            }

            if (_value == 0)
            {
                return "0";
            }

            return _value.ToString("#");
        }

    } // class
} // namespace

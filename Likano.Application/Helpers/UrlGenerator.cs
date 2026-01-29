namespace Likano.Application.Helpers
{
    public static class UrlGenerator
    {
        private static readonly string geo = "აბგდევზთიკლმნოპჟრსტუფქღყშჩცძწჭხჯჰ";
        private static readonly string[] lat = "a-b-g-d-e-v-z-t-i-k-l-m-n-o-p-zh-r-s-t-u-f-q-gh-y-sh-ch-c-dz-ts-tch-x-j-h".Split('-');

        public static string Transliterate(string? text)
        {
            var result = "";
            if (!string.IsNullOrWhiteSpace(text))
            {
                foreach (var ch in text.Trim())
                {
                    var index = geo.IndexOf(ch);
                    result += index != -1 ? lat[index] : ch.ToString();
                }
                result = result.Replace(' ', '-');
                result = result.Replace('&', '-');
                result = result.Replace('/', '-');
                result = result.Replace('\\', '-');
                result = result.Replace('#', '-');
                result = result.Replace('%', '-');
            }
            return result;
        }
    }
}

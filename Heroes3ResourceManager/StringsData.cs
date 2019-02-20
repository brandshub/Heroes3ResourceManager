using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public class StringsData
    {
        private const string FNAME_JKTEXT = "JKTEXT.TXT";

        public string[] JKTEXT;

        public StringsData(H3Bitmap lodFile)
        {
            var rec = lodFile.GetRecord(FNAME_JKTEXT);
            JKTEXT = Encoding.Default.GetString(rec.GetRawData(lodFile.stream)).Split(new string[] { "\r\n" }, StringSplitOptions.None);
        }
    }
}

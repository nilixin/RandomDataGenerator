namespace RandGen
{
    public partial class Form1 : Form
    {
        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void bGenerate_Click(object sender, EventArgs e)
        {
            tbOutput.Text = null;
            string input = tbInput.Text.Replace("\r", "");
            string query = query = tbBefore.Text;
            string after = tbAfter.Text;

            try
            {
                for (int i = 0; i < nudIterations.Value; i++)
                {
                    Fill(input, query, after);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Fill(string input, string query, string after)
        {
            List<string> atts = input.Split('\n').ToList();
            for (int j = 0; j < atts.Count; j++) // for each attribute in attributes
            {
                if (atts[j].Contains("/##")) // if "##" at the beginning of a line
                {
                    atts[j] = atts[j].Replace("/##", "");
                    query = NewAttribute(query, "\"", atts[j]);
                    if (j != atts.Count - 1)
                        query += ",";
                }
                else if (atts[j].Contains("/#")) // if "#" at the beginning of a line
                {
                    atts[j] = atts[j].Replace("/#", "");
                    query = NewAttribute(query, "'", atts[j]);
                    if (j != atts.Count - 1)
                        query += ",";
                }
                else
                {
                    query = NewAttribute(query, "", atts[j]);
                }
            }

            query = query.Replace("  ", " ");
            query = query.Replace(" '", "'");
            query = query.Replace("' ", "'");
            //query = query.Replace(" ,", ",");
            //query = query.Replace("/#", "");
            //query = query.Replace("/##", "");
            //query = query.Replace("/_", "");
            //query = query.Replace("/_", "");

            tbOutput.Text += query + after + "\r\n";
        }

        private string NewAttribute(string query, string wrapper, string line)
        {
            query += wrapper;

            List<string> opts = line.Split('|').ToList();
            int count = opts.Count;
            for (int k = 0; k < count; k++) // for each option in options
            {
                bool containsSpaceBefore = opts[k].Contains("/__");
                bool containsSpaceAfter = false;
                if (!containsSpaceBefore)
                    containsSpaceAfter = opts[k].Contains("/_");
                opts[k] = opts[k].Replace("/__", "");
                opts[k] = opts[k].Replace("/_", "");

                if (containsSpaceBefore)
                    query += " ";

                query = NewOption(query, opts[k], count);

                if (containsSpaceAfter)
                    query += " ";
            }

            query += wrapper;
            
            return query;
        }

        private string NewOption(string query, string option, int count)
        {
            List<string> vals = option.Split(';').ToList();
            int index = random.Next(0, vals.Count);
            string val = vals[index];
            query += val;
            
            return query;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JMW.Template.Tags
{
    public class Output : TagHandlerBase
    {
        private Dictionary<string, int> _iteration = new Dictionary<string, int>();

        public const string TAG = "output";
        public override string TagName { get; } = TAG;

        #region Properties

        public override HashSet<string> ALLOWEDPROPS { get; } = new HashSet<string> { ATTR_EXCEL, ATTR_FILENAME, ATTR_MODE };

        public const string ATTR_FILENAME = "filename";

        public const string ATTR_EXCEL = VALUE_EXCEL_COLUMN;
        public const string VALUE_EXCEL_SHEET = "sheet";
        public const string VALUE_EXCEL_COLUMN = "column";

        public const string ATTR_EXCEL_NAME = "excel_name";

        public const string ATTR_MODE = "mode";
        public const string VALUE_MODE_NEW = "new";
        public const string VALUE_MODE_APPEND = "append";

        #endregion Properties

        public Dictionary<string, StringBuilder> Outputs { get; } = new Dictionary<string, StringBuilder>();
        public bool RedirectOutputs { get; set; } = false;

        public override void Handler(Tag tag, Interpreter interp)
        {
            if (!tag.Properties.ContainsKey(ATTR_FILENAME))
                throw new Exception("the filename attribute is required.");

            var stream = interp.OutputStream;

            // create new stream and handle properties.
            var is_excel = tag.Properties.ContainsKey(ATTR_EXCEL);
            var mode = tag.Properties.ContainsKey(ATTR_MODE) ? tag.Properties[ATTR_MODE] : VALUE_MODE_APPEND;

            if (is_excel)
            {
                var type = tag.Properties[ATTR_EXCEL];
                switch (type)
                {
                    case VALUE_EXCEL_COLUMN:
                        break;

                    case VALUE_EXCEL_SHEET:
                        break;
                }
            }
            else
            {
                switch (mode)
                {
                    case VALUE_MODE_NEW:
                    {
                        var file = tag.Properties[ATTR_FILENAME];
                        var ext = Path.GetExtension(file);
                        var file_name = Path.GetFileNameWithoutExtension(file);

                        if (!_iteration.ContainsKey(file))
                            _iteration.Add(file, 0);

                        var new_file_name = file_name + _iteration[file] + ext;

                        if (RedirectOutputs)
                        {
                            Outputs.Add(TAG + _iteration[file], new StringBuilder());
                            interp.OutputStream = new StringWriter(Outputs[TAG + _iteration[file]]);
                        }
                        else
                        {
                            interp.OutputStream = new StreamWriter(new FileStream(new_file_name, FileMode.OpenOrCreate, FileAccess.ReadWrite));
                        }

                        _iteration[file]++;
                        break;
                    }
                    case VALUE_MODE_APPEND:
                    {
                        if (RedirectOutputs)
                        {
                            if (Outputs.Count == 0)
                                Outputs.Add(TAG, new StringBuilder());

                            interp.OutputStream = new StringWriter(Outputs[TAG]);
                        }
                        else
                        {
                            interp.OutputStream = new StreamWriter(new FileStream(tag.Properties[ATTR_FILENAME], FileMode.Append, FileAccess.Write));
                        }
                        break;
                    }
                    default:
                        throw new Exception("Unknown attribute file for " + ATTR_MODE + ".  Allowed values are: " + VALUE_MODE_NEW + " and " + VALUE_MODE_APPEND);
                }
            }

            interp.HandleChildren(tag);

            interp.OutputStream.Close();
            interp.OutputStream = stream;
        }

        public static bool IsOutput(Tag tag)
        {
            return tag.TagType == TagTypes.Tag && tag.Name == TAG;
        }
    }

    public enum ExcelOutputTypes
    {
        FILE,
        SHEET,
        COLUMN
    }
}
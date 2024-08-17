using System.Text;

namespace ReswPlus.Core.CodeGenerators
{
    public class CodeStringBuilder
    {
        private readonly StringBuilder _stringBuilder;
        private readonly string _indentString;
        private uint _level;

        public CodeStringBuilder(string indentString)
        {
            _level = 0;
            _stringBuilder = new StringBuilder();
            _indentString = indentString;
        }

        public CodeStringBuilder AppendLine(string value)
        {
            AddSpace(_level);
            _stringBuilder.AppendLine(value);
            return this;
        }

        private void AddSpace(uint level)
        {
            for (var i = 0; i < level; ++i)
            {
                _stringBuilder.Append(_indentString);
            }
        }

        public CodeStringBuilder AddLevel()
        {
            ++_level;
            return this;
        }

        public CodeStringBuilder RemoveLevel()
        {
            if (_level > 0)
            {
                --_level;
            }
            return this;
        }

        public CodeStringBuilder AppendEmptyLine()
        {
            _stringBuilder.AppendLine("");
            return this;
        }

        public string GetString()
        {
            return _stringBuilder.ToString();
        }
    }
}

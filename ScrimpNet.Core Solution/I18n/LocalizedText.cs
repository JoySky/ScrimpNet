using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace ScrimpNet.Core.i18N
{

    /// <summary>
    /// Represents a single bit of text that could be localized.  A text is represented by 1..N Variants that contain culture/text mappings
    /// </summary>
    [Serializable]
    public class LocalizedText : IComparable, IXmlSerializable, IFormattable 
    {
        private static CultureCode _activeCulture = CultureCode.Unknown;

        /// <summary>
        /// Class level active culture code.  Used if object does not have a specific culture set.  Usually not specifically set.
        /// </summary>
        public static CultureCode ActiveCulture
        {
            get {
                if (_activeCulture == CultureCode.Unknown)
                {
                    return (CultureCode)Enum.Parse(typeof(CultureCode),Utils.CurrentLocale.Replace("-", "_"));
                }
                return _activeCulture; 
            }
            set { _activeCulture = value; }
        }
        
        

        private static object lockObject = new object();
        static LocalizedText()
        {

        }
        /// <summary>
        /// Sets the class level culture.  Set to CultureCode.Unknown to allow culture to come from Current Thread
        /// </summary>
        /// <param name="newCulture">New culture for this class</param>
        /// <returns>Previous code</returns>
        public static CultureCode SetActiveCulture(CultureCode newCulture)
        {
            CultureCode oldCode = _activeCulture;
            lock (lockObject)
            {
                ActiveCulture = newCulture;
            }
            return oldCode;
        }

        /// <summary>
        /// Represents a single text variant that for this text item.  A text item might have two entries each culture (Roman and Script)
        /// </summary>
        public struct Variant
        {
            /// <summary>
            /// Actual unicode text
            /// </summary>
            public string Text;

            /// <summary>
            /// CultureCode for this text entry
            /// </summary>
            public CultureCode LanguageIdentifier;

            /// <summary>
            /// True if this text is romanized.  False if in linguistic normal script.  For Romanized languages (English etc) will be True
            /// </summary>
            public bool IsRoman;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="text">Text to assign</param>
            /// <param name="culture">Culture to bind to this text</param>
            public Variant(string text, CultureCode culture)
            {
                IsRoman = true;
                Text = text;
                LanguageIdentifier = culture;
            }

            /// <summary>
            /// Create a text entry that is bound to ActiveCulture of this class.  Default to Roman
            /// </summary>
            /// <param name="text">Text to localize</param>
            public Variant(string text)
            {
                Text = text;
                LanguageIdentifier = ActiveCulture;
                IsRoman = true;
            }

            /// <summary>
            /// Copy constructor
            /// </summary>
            /// <param name="variant">Variant element to copy</param>
            public Variant(Variant variant)
            {
                this.Text = variant.Text;
                this.LanguageIdentifier = variant.LanguageIdentifier;
                this.IsRoman = variant.IsRoman;            
            }
            /// <summary>
            /// Create a copy of a variant
            /// </summary>
            /// <param name="variant">Variant to copy</param>
            /// <returns>Copy of parameter varaint</returns>
            public static Variant NewVariant(Variant variant)
            {
                return Utils.Clone<Variant>(variant);
            }
        } //struct Variant

        private string _resourceKey;
        private Dictionary<CultureCode, string> _textItems = new Dictionary<CultureCode, string>();
        private int _resourceId;

        /// <summary>
        /// Key for finding this resource in localized storage (resource file or database table).  First part of compound key
        /// (e.g. CUR, CTRY, REGION)
        /// </summary>
        public string ResourceKey
        {
            get { return _resourceKey; }
            set { _resourceKey = value; }
        }

        /// <summary>
        /// Numeric identifier.  Second part of compound key with ResourceKey
        /// </summary>
        public int ResourceId
        {
            get { return _resourceId; }
            set { _resourceId = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LocalizedText()
        {
            _resourceKey = string.Empty;
        }

        /// <summary>
        /// Constructor.  Sets text using LocalizedText.ActiveCulture
        /// </summary>
        /// <param name="text">Text to assign to this instance</param>
        public LocalizedText(string text)
        {
            SetText(text, LocalizedText.ActiveCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public string this[CultureCode cultureCode]
        {
            get
            {
                return this[cultureCode, true];
            }
        }

        public string this[CultureCode cultureCode, bool returnDefaultCulture]
        {
            get
            {
                if (_textItems.ContainsKey(cultureCode) == true)
                {
                    return _textItems[cultureCode];
                }
                if (returnDefaultCulture == true)
                {
                    return this[LocalizedText.ActiveCulture, false];
                }
                throw new IndexOutOfRangeException(string.Format("Unable to find text for CultureCode: '{0}'", cultureCode));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Text assign to this instance</param>
        /// <param name="resourceKey">Key into resource storage (resource file or database store)</param>
        /// <param name="cultureCode">Culture code for this text</param>
        public LocalizedText(string text, string resourceKey, CultureCode cultureCode)
        {
            SetText(text, resourceKey, cultureCode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Text assign to this instance</param>
        /// <param name="cultureCode">Culture code for this text</param>
        public LocalizedText(string text, CultureCode cultureCode)
        {
            SetText(text, cultureCode);
        }

        /// <summary>
        /// Overloaded ToString() operator.  Calls custom formatter
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString("t", null);
        }

        /// <summary>
        /// Set text on this instance
        /// </summary>
        /// <param name="text">Text to assign to this instance</param>
        /// <param name="cultureCode">Culture code of this text</param>
        public void SetText(string text, CultureCode cultureCode)
        {
            _textItems[cultureCode] = text;
        }

        /// <summary>
        /// Set text on this instance
        /// </summary>
        /// <param name="text">Text to assign to this instance</param>
        /// <param name="resourceKey">Key for finding this resource in localized storage (resource file or database table)</param>
        /// <param name="cultureCode">Culture code for this text</param>
        public void SetText(string text, string resourceKey, CultureCode cultureCode)
        {
            SetText(text, cultureCode);
            ResourceKey = resourceKey;
        }


        public string Text
        {
            get
            {
                return GetText(ActiveCulture).Text;
            }
        }
        
        public Variant GetText()
        {
            return GetText(ActiveCulture);
        }

        public Variant GetText(CultureCode cultureCode)
        {
            Variant v = new Variant();
            return v;
        }

        /// <summary>
        /// Returns the all localized text within this collection
        /// </summary>
        /// <returns></returns>
        public Variant[] GetAllVariants()
        {
            Variant[] retval = new Variant[_textItems.Count];
            int x = 0;
            foreach (KeyValuePair<CultureCode, string> kvp in _textItems)
            {
                retval[x++] = new Variant(kvp.Value, kvp.Key);
            }
            return retval;
        }

        public string CompoundKey
        {
            get
            {
                return this.ResourceKey + this.ResourceId.ToString();
            }
        }
        #region IComparable Members

        /// <summary>
        /// Compare two strings
        /// </summary>
        /// <param name="obj">Localized text to compare</param>
        /// <returns>-1 if this &lt; obj; 0 if this == obj; 1 if this &gt; obj</returns>
        public int CompareTo(object obj)
        {
            return this[LocalizedText.ActiveCulture, true].CompareTo(obj);
        }

        #endregion


        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("ReadXML Not Implemented");
        }



        #endregion

        #region IXmlSerializable Members


        public void WriteXml(XmlWriter writer)
        {
            //            writer.WriteStartElement("localizedText");
            writer.WriteAttributeString("key", this.CompoundKey);
            writer.WriteElementString("resourceKey", ResourceKey);
            writer.WriteElementString("resourceId", ResourceId.ToString());
            writer.WriteStartElement("variants");


            Variant[] v = GetAllVariants();
            foreach (Variant var in v)
            {
                writer.WriteStartElement("variant");
                writer.WriteAttributeString("languageIdentifer", var.LanguageIdentifier.ToString());
                writer.WriteAttributeString("isRoman", var.IsRoman.ToString());
                writer.WriteString(var.Text);
                writer.WriteEndElement(); //variant
            }

            writer.WriteEndElement(); //variants
            //          writer.WriteEndElement(); //localized text
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IFormattable Members

     
        /// <summary>
        /// Custom provider that allows easier access to a particular lingual variant of the text
        /// </summary>
        /// <param name="format">Argument passed by String.Format call.  Part of {0:t} that is after ':'</param>
        /// <param name="formatProvider">Argument passed by String.Format call</param>
        /// <returns>Linguistic variant of text that matches the culture code or String.Empty if not found</returns>
        /// <remarks>
        /// <para>{0:t}   (default) probe for matching variant: filters, ActiveCulture(script,roman), ClassCulture(script,roman), LibaryCulture(script,roman), ThreadUICulture, ThreadCulture</para>
        /// <para>{0:G}   same as {0:t}</para>
        /// <para>{0:tr}  probe for matching variant using standard probing rules but seek roman text first then seek script</para>
        /// <para>{0:ts}  probe for matching variant using standard probing rules but seek script text first then seek Roman</para>
        /// <para>{0:tR}  probe for matching variant using standard probing rules but seek <b>only</b> Roman text, return String.Empty if not found</para>
        /// <para>{0:tS}  probe for matching variant using standard probing rules but seek <b>only</b> script text, return String.Empty if not found</para>
        /// <para>{0:T}   seek matching variant using only ActiveCulture set in this object; script first then Roman.  Same as TS</para>
        /// <para>{0:Tr}  seek matching variant using only ActiveCulture set in this object; Roman first then script</para>
        /// <para>{0:Ts}  seek matching variant using only ActiveCulture set in this object; script first then Roman</para>
        /// <para>{0:TR}  seek matching variant using only ActiveCulture set in this object; Roman only or return String.Empty if not found</para>
        /// <para>{0:TS}  seek matching variant using only ActiveCulture set in this object; Script only or return String.Empty if not found</para>
        /// <para></para>
        /// <para><b>Culture Code Filters</b></para>
        /// <para>Culture code filters are a semi-colon delimited list of cultures and provide a way to retrieve an explicit variant text.
        /// Filters are generally evaluated before an other probing or formatting rules.</para>
        /// <para>{0:t,en-US;en;es-US}  Look for text (script then roman variants) that match these cultures before proceeding the default behavior</para>
        /// </remarks>
        public string ToString(string format, IFormatProvider formatProvider)
        {            
            string[] aFilters = format.Split(new char[] {','});

            return this[CultureCode.fr_CA];
        }

        #endregion
    } //class LocalizedText

} //namespace


using System.Collections.Generic;
using System.Linq;

namespace ReswPlus.SourceGenerator.ClassGenerators;

/// <summary>
/// Provides functionality to manage and retrieve pluralization rules for various languages.
/// </summary>
internal class PluralFormsRetriever
{
    internal record PluralForm
    {
        public string[] Languages { get; set; }
        public string Id { get; set; }
    }

    /// <summary>
    /// A static collection of predefined plural forms and their associated languages.
    /// </summary>
    private static readonly PluralForm[] PluralForms = [
        new () {
            Languages = [
                "ak", // Akan
			        "bh", // Bihari
			        "guw", // Gun
			        "ln", // Lingala
			        "mg", // Malagasy
			        "nso", // Northern Sotho
			        "pa", // Punjabi
			        "ti", // Tigrinya
			        "wa" // Walloon
		        ],
            Id = "IntOneOrZero"
        },
        new () {
            Languages = [
                "am", // Amharic
			        "bn", // Bengali
			        "ff", // Fulah
			        "gu", // Gujarati
			        "hi", // Hindi
			        "kn", // Kannada
			        "mr", // Marathi
			        "fa", // Persian
			        "zu", // Zulu
		        ],
            Id = "ZeroToOne"
        },
        new () {
            Languages = [
                "hy", // Armenian
			        "fr", // French
			        "kab", // Kabyle
		        ],
            Id = "ZeroToTwoExcluded"
        }

        ,
        new () {
            Languages = [
                "af", // Afrikaans
			        "sq", // Albanian
			        "ast", // Asturian
			        "asa", // Asu
			        "az", // Azerbaijani
			        "eu", // Basque
			        "bem", // Bemba
			        "bez", // Bena
			        "brx", // Bodo
			        "bg", // Bulgarian
			        "ca", // Catalan
			        "chr", // Cherokee
			        "cgg", // Chiga
			        "dv", // Divehi
			        "nl", // Dutch
			        "en", // English
			        "eo", // Esperanto
			        "et", // Estonian
			        "pt", // European Portuguese
			        "ee", // Ewe
			        "fo", // Faroese
			        "fi", // Finnish
			        "fur", // Friulian
			        "gl", // Galician
			        "lg", // Ganda
			        "ka", // Georgian
			        "de", // German
			        "el", // Greek
			        "ha", // Hausa
			        "haw", // Hawaiian
			        "hu", // Hungarian
			        "it", // Italian
			        "kaj", // Jju
			        "kkj", // Kako
			        "kl", // Kalaallisut
			        "ks", // Kashmiri
			        "kk", // Kazakh
			        "ku", // Kurdish
			        "ky", // Kyrgyz
			        "lb", // Luxembourgish
			        "jmc", // Machame
			        "ml", // Malayalam
			        "mas", // Masai
			        "mgo", // Meta'
			        "mn", // Mongolian
			        "nah", // Nahuatl
			        "ne", // Nepali
			        "nnh", // Ngiemboon
			        "jgo", // Ngomba
			        "nd", // North Ndebele
			        "no", // Norwegian
			        "nb", // Norwegian Bokmål
			        "nn", // Norwegian Nynorsk
			        "ny", // Nyanja
			        "nyn", // Nyankole
			        "or", // Oriya
			        "om", // Oromo
			        "os", // Ossetic    
			        "pap", // Papiamento
			        "ps", // Pashto
			        "rm", // Romansh
			        "rof", // Rombo
			        "rwk", // Rwa
			        "ssy", // Saho
			        "sag", // Samburu
			        "seh", // Sena
			        "ksb", // Shambala
			        "sn", // Shona
			        "xog", // Soga
			        "so", // Somali
			        "ckb", // Sorani Kurdish
			        "nr", // South Ndebele
			        "st", // Southern Sotho
			        "es", // Spanish
			        "sw", // Swahili
			        "ss", // Swati
			        "sv", // Swedish
			        "gsw", // Swiss German
			        "syr", // Syriac
			        "ta", // Tamil
			        "te", // Telugu
			        "teo", // Teso
			        "tig", // Tigre
			        "ts", // Tsonga
			        "tn", // Tswana
			        "tr", // Turkish
			        "tk", // Turkmen
			        "kcg", // Tyap
			        "ur", // Urdu
			        "ug", // Uyghur
			        "uz", // Uzbek
			        "ve", // Venda
			        "vo", // Volapük
			        "vun", // Vunjo
			        "wae", // Walser
			        "fy", // Western Frisian
			        "xh", // Xhosa
			        "yi", // Yiddish
			        "ji", // ji
		        ],
            Id = "OnlyOne"
        },
        new () {
            Languages = [
                "si", // Sinhala
		        ],
            Id = "Sinhala"
        },
        new () {
            Languages = [
                "lv", // Latvian 
			        "prg", // Prussian
		        ],
            Id = "Latvian"
        },
        new () {
            Languages = [
                "ga", // Irish
		        ],
            Id = "Irish"
        },
        new () {
            Languages = [
                "ro", // Romanian
			        "mo", // Moldavian
		        ],
            Id = "Romanian"
        },
        new () {
            Languages = [
                "lt", // Lithuanian
		        ],
            Id = "Lithuanian"
        },
        new () {
            Languages = [
                "ru", // Russian
			        "uk", // Ukrainian
			        "be", // Belarusian
		        ],
            Id = "Slavic"
        },
        new () {
            Languages = [
                "cs", // Czech
			        "sk", // Slovak
		        ],
            Id = "Czech"
        },
        new () {
            Languages = [
                "pl", // Polish 
		        ],
            Id = "Polish"
        },
        new () {
            Languages = [
                "sl", // Slovenian
		        ],
            Id = "Slovenian"
        },
        new () {
            Languages = [
                "ar", // Arabic
		        ],
            Id = "Arabic"
        },
        new () {
            Languages = [
                "he", // Hebrew
			        "iw", // Iw
		        ],
            Id = "Hebrew"
        },
        new () {
            Languages = [
                "fil", // Filipino
			        "tl", // Tagalog
		        ],
            Id = "Filipino"
        },
        new () {
            Languages = [
                "mk",
            ],
            Id = "Macedonian"
        },
        new () {
            Languages = [
                "br", // Breton
		        ],
            Id = "Breizh"
        },
        new () {
            Languages = [
                "tzm", // Central Atlas Tamazight
		        ],
            Id = "CentralAtlasTamazight"
        },
        new () {
            Languages = [
                "ksh", //Colognian
		        ],
            Id = "OneOrZero"
        },
        new () {
            Languages = [
                "lag", //Langi
		        ],
            Id = "OneOrZeroToOneExcluded"
        },
        new () {
            Languages = [
                "kw", // Cornish
			        "smn", // Inari Sami
			        "iu", // Inuktitut
			        "smj", // Lule Sami
			        "naq", // Nama
			        "se", // Northern Sami
			        "smi", // Sami languages [Other]
			        "sms", // Skolt Sami
			        "sma", // Southern Sami
		        ],
            Id = "OneOrTwo"
        },
        new () {
            Languages = [
                "bs", // Bosnian
			        "hr", // Croatian
			        "sr", // Serbian
			        "sh", // Serbo-Croatian
		        ],
            Id = "Croat"
        },
        new () {
            Languages = [
                "shi",
            ],
            Id = "Tachelhit"
        },
        new () {
            Languages = [
                "is",
            ],
            Id = "Icelandic"
        },
        new () {
            Languages = [
                "gv",
            ],
            Id = "Manx"
        },
        new () {
            Languages = [
                "gd",
            ],
            Id = "ScottishGaelic"
        },
        new () {
            Languages = [
                "mt", // Maltese
		        ],
            Id = "Maltese"
        },
        new () {
            Languages = [
                "cy", // Welsh
		        ],
            Id = "Welsh"
        },
        new () {
            Languages = [
                "da", // Danish
		        ],
            Id = "Danish"
        }
    ];

    /// <summary>
    /// Retrieves the plural forms that apply to the given list of languages.
    /// </summary>
    /// <param name="languages">A collection of language codes to retrieve plural forms for.</param>
    /// <returns>An enumerable collection of <see cref="PluralForm"/> objects that match the specified languages.</returns>
    public static IEnumerable<PluralForm> RetrievePluralFormsForLanguages(IEnumerable<string> languages)
    {
        foreach (var pluralForm in PluralForms)
        {
            var shortenLanguagesList = pluralForm.Languages.Intersect(languages).ToArray();
            if (shortenLanguagesList.Any())
            {
                yield return new PluralForm()
                {
                    Id = pluralForm.Id,
                    Languages = shortenLanguagesList
                };
            }
        }
    }
}

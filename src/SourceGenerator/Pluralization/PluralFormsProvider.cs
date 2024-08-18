using System.Collections.Generic;
using System.Linq;

namespace ReswPlusSourceGenerator.Pluralization;

/// <summary>
/// Provides functionality to manage and retrieve pluralization rules for various languages.
/// </summary>
internal class PluralFormsProvider
{
    /// <summary>
    /// A static collection of predefined plural forms and their associated languages.
    /// </summary>
    private static readonly PluralForm[] PluralForms = new PluralForm[] {
        new () {
            Languages = new [] {
                "ak", // Akan
			        "bh", // Bihari
			        "guw", // Gun
			        "ln", // Lingala
			        "mg", // Malagasy
			        "nso", // Northern Sotho
			        "pa", // Punjabi
			        "ti", // Tigrinya
			        "wa" // Walloon
		        },
            Id = "IntOneOrZero"
        },
        new () {
            Languages = new [] {
                "am", // Amharic
			        "bn", // Bengali
			        "ff", // Fulah
			        "gu", // Gujarati
			        "hi", // Hindi
			        "kn", // Kannada
			        "mr", // Marathi
			        "fa", // Persian
			        "zu", // Zulu
		        },
            Id = "ZeroToOne"
        },
        new () {
            Languages = new [] {
                "hy", // Armenian
			        "fr", // French
			        "kab", // Kabyle
		        },
            Id = "ZeroToTwoExcluded"
        }

        ,
        new () {
            Languages = new [] {
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
		        },
            Id = "OnlyOne"
        },
        new () {
            Languages = new [] {
                "si", // Sinhala
		        },
            Id = "Sinhala"
        },
        new () {
            Languages = new [] {
                "lv", // Latvian 
			        "prg", // Prussian
		        },
            Id = "Latvian"
        },
        new () {
            Languages = new [] {
                "ga", // Irish
		        },
            Id = "Irish"
        },
        new () {
            Languages = new [] {
                "ro", // Romanian
			        "mo", // Moldavian
		        },
            Id = "Romanian"
        },
        new () {
            Languages = new [] {
                "lt", // Lithuanian
		        },
            Id = "Lithuanian"
        },
        new () {
            Languages = new [] {
                "ru", // Russian
			        "uk", // Ukrainian
			        "be", // Belarusian
		        },
            Id = "Slavic"
        },
        new () {
            Languages = new [] {
                "cs", // Czech
			        "sk", // Slovak
		        },
            Id = "Czech"
        },
        new () {
            Languages = new [] {
                "pl", // Polish 
		        },
            Id = "Polish"
        },
        new () {
            Languages = new [] {
                "sl", // Slovenian
		        },
            Id = "Slovenian"
        },
        new () {
            Languages = new [] {
                "ar", // Arabic
		        },
            Id = "Arabic"
        },
        new () {
            Languages = new [] {
                "he", // Hebrew
			        "iw", // Iw
		        },
            Id = "Hebrew"
        },
        new () {
            Languages = new [] {
                "fil", // Filipino
			        "tl", // Tagalog
		        },
            Id = "Filipino"
        },
        new () {
            Languages = new [] {
                "mk",
            },
            Id = "Macedonian"
        },
        new () {
            Languages = new [] {
                "br", // Breton
		        },
            Id = "Breizh"
        },
        new () {
            Languages = new [] {
                "tzm", // Central Atlas Tamazight
		        },
            Id = "CentralAtlasTamazight"
        },
        new () {
            Languages = new [] {
                "ksh", //Colognian
		        },
            Id = "OneOrZero"
        },
        new () {
            Languages = new [] {
                "lag", //Langi
		        },
            Id = "OneOrZeroToOneExcluded"
        },
        new () {
            Languages = new [] {
                "kw", // Cornish
			        "smn", // Inari Sami
			        "iu", // Inuktitut
			        "smj", // Lule Sami
			        "naq", // Nama
			        "se", // Northern Sami
			        "smi", // Sami languages [Other]
			        "sms", // Skolt Sami
			        "sma", // Southern Sami
		        },
            Id = "OneOrTwo"
        },
        new () {
            Languages = new [] {
                "bs", // Bosnian
			        "hr", // Croatian
			        "sr", // Serbian
			        "sh", // Serbo-Croatian
		        },
            Id = "Croat"
        },
        new () {
            Languages = new [] {
                "shi",
            },
            Id = "Tachelhit"
        },
        new () {
            Languages = new [] {
                "is",
            },
            Id = "Icelandic"
        },
        new () {
            Languages = new [] {
                "gv",
            },
            Id = "Manx"
        },
        new () {
            Languages = new [] {
                "gd",
            },
            Id = "ScottishGaelic"
        },
        new () {
            Languages = new [] {
                "mt", // Maltese
		        },
            Id = "Maltese"
        },
        new () {
            Languages = new [] {
                "cy", // Welsh
		        },
            Id = "Welsh"
        },
        new () {
            Languages = new [] {
                "da", // Danish
		        },
            Id = "Danish"
        }
    };

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

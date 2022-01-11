using System;
using System.Collections.Generic;
using System.Text;

namespace ReswPlusSourceGenerator
{
    internal class Pluralizations
    {
        public static readonly PluralForm[] PluralForms = new[] {
            new PluralForm() {
                Languages = new string[] {
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
                Name = "IntOneOrZero"
            },
            new PluralForm() {
                Languages = new string[] {
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
                Name = "ZeroToOne"
            },
            new PluralForm() {
                Languages = new string[] {
                    "hy", // Armenian
			        "fr", // French
			        "kab", // Kabyle
		        },
                Name = "ZeroToTwoExcluded"
            }

            ,
            new PluralForm() {
                Languages = new string[] {
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
                Name = "OnlyOne"
            },
            new PluralForm() {
                Languages = new string[] {
                    "si", // Sinhala
		        },
                Name = "Sinhala"
            },
            new PluralForm() {
                Languages = new string[] {
                    "lv", // Latvian 
			        "prg", // Prussian
		        },
                Name = "Latvian"
            },
            new PluralForm() {
                Languages = new string[] {
                    "ga", // Irish
		        },
                Name = "Irish"
            },
            new PluralForm() {
                Languages = new string[] {
                    "ro", // Romanian
			        "mo", // Moldavian
		        },
                Name = "Romanian"
            },
            new PluralForm() {
                Languages = new string[] {
                    "lt", // Lithuanian
		        },
                Name = "Lithuanian"
            },
            new PluralForm() {
                Languages = new string[] {
                    "ru", // Russian
			        "uk", // Ukrainian
			        "be", // Belarusian
		        },
                Name = "Slavic"
            },
            new PluralForm() {
                Languages = new string[] {
                    "cs", // Czech
			        "sk", // Slovak
		        },
                Name = "Czech"
            },
            new PluralForm() {
                Languages = new string[] {
                    "pl", // Polish 
		        },
                Name = "Polish"
            },
            new PluralForm() {
                Languages = new string[] {
                    "sl", // Slovenian
		        },
                Name = "Slovenian"
            },
            new PluralForm() {
                Languages = new string[] {
                    "ar", // Arabic
		        },
                Name = "Arabic"
            },
            new PluralForm() {
                Languages = new string[] {
                    "he", // Hebrew
			        "iw", // Iw
		        },
                Name = "Hebrew"
            },
            new PluralForm() {
                Languages = new string[] {
                    "fil", // Filipino
			        "tl", // Tagalog
		        },
                Name = "Filipino"
            },
            new PluralForm() {
                Languages = new string[] {
                    "mk",
                },
                Name = "Macedonian"
            },
            new PluralForm() {
                Languages = new string[] {
                    "br", // Breton
		        },
                Name = "Breizh"
            },
            new PluralForm() {
                Languages = new string[] {
                    "tzm", // Central Atlas Tamazight
		        },
                Name = "CentralAtlasTamazight"
            },
            new PluralForm() {
                Languages = new string[] {
                    "ksh", //Colognian
		        },
                Name = "OneOrZero"
            },
            new PluralForm() {
                Languages = new string[] {
                    "lag", //Langi
		        },
                Name = "OneOrZeroToOneExcluded"
            },
            new PluralForm() {
                Languages = new string[] {
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
                Name = "OneOrTwo"
            },
            new PluralForm() {
                Languages = new string[] {
                    "bs", // Bosnian
			        "hr", // Croatian
			        "sr", // Serbian
			        "sh", // Serbo-Croatian
		        },
                Name = "Croat"
            },
            new PluralForm() {
                Languages = new string[] {
                    "shi",
                },
                Name = "Tachelhit"
            },
            new PluralForm() {
                Languages = new string[] {
                    "is",
                },
                Name = "Icelandic"
            },
            new PluralForm() {
                Languages = new string[] {
                    "gv",
                },
                Name = "Manx"
            },
            new PluralForm() {
                Languages = new string[] {
                    "gd",
                },
                Name = "ScottishGaelic"
            },
            new PluralForm() {
                Languages = new string[] {
                    "mt", // Maltese
		        },
                Name = "Maltese"
            },
            new PluralForm() {
                Languages = new string[] {
                    "cy", // Welsh
		        },
                Name = "Welsh"
            },
            new PluralForm() {
                Languages = new string[] {
                    "da", // Danish
		        },
                Name = "Danish"
            }
        };
    }
}

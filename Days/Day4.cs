using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using static AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Days
{
    public class Day4
    {
//         private static string _ = @"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
// byr:1937 iyr:2017 cid:147 hgt:183cm

// iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
// hcl:#cfa07d byr:1929

// hcl:#ae17e1 iyr:2013
// eyr:2024
// ecl:brn pid:760753108 byr:1931
// hgt:179cm

// hcl:#cfa07d eyr:2025 pid:166559648
// iyr:2011 ecl:brn hgt:59in";

//         private static string __ = @"eyr:1972 cid:100
// hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

// iyr:2019
// hcl:#602927 eyr:1967 hgt:170cm
// ecl:grn pid:012533040 byr:1946

// hcl:dab227 iyr:2012
// ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

// hgt:59cm ecl:zzz
// eyr:2038 hcl:74454a iyr:2023
// pid:3556412378 byr:2007

// pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
// hcl:#623a2f

// eyr:2029 ecl:blu cid:129 byr:1989
// iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

// hcl:#888785
// hgt:164cm byr:2001 iyr:2015 cid:88
// pid:545766238 ecl:hzl
// eyr:2022

// iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719";

        private static string _input = File.ReadAllText(@"Days\Inputs\Day4.txt");

        private static IEnumerable<Passport> ProcessInput(string input, bool useExtendedValidation)
        {
            return input.Split($"{Environment.NewLine}{Environment.NewLine}")
                        .Select(s => Passport.Parse(s, useExtendedValidation));
        }

        public static int Problem1()
        {
            return ProcessInput(_input, false).Count(p => p.IsValid);
        }

        public static int Problem2()
        {
            return ProcessInput(_input, true).Count(p => p.IsValid);
        }

        private record Passport
        {
            public int? BirthYear { get; private set; }
            public int? IssueYear { get; private set; }
            public int? ExpirationYear { get; private set; }

#nullable enable
            public string? Height { get; private set; }
            public string? HairColor { get; private set; }
            public EyeColor EyeColor { get; private set; }
            public string? PassportId { get; private set; }
#nullable disable

            public int? CountryId { get; private set; }

            public bool IsValid => UseExtendedValidation
                ? IsBasicallyValid
                    && BirthYear.Includes(1920, 2002)
                    && ExpirationYear.Includes(2020, 2030)
                    && IssueYear.Includes(2010, 2020)
                    && IsHeightValid
                    && IsHairColorValid
                    && IsPassportIdValid
                : IsBasicallyValid;

            private bool IsBasicallyValid => BirthYear.HasValue
                                          && IssueYear.HasValue
                                          && ExpirationYear.HasValue
                                          && !string.IsNullOrEmpty(Height)
                                          && !string.IsNullOrEmpty(HairColor)
                                          && EyeColor != EyeColor.Unspecified
                                          && !string.IsNullOrEmpty(PassportId);

            private bool UseExtendedValidation { get; set; }

            public override string ToString()
            {
                return "Pasport Properties:"
                    + $"{Environment.NewLine}\tBirth Year: {BirthYear}"
                    + $"{Environment.NewLine}\tIssue Year: {IssueYear}"
                    + $"{Environment.NewLine}\tExpiration Year: {ExpirationYear}"
                    + $"{Environment.NewLine}\tHeight: {Height}"
                    + $"{Environment.NewLine}\tHair Color: {HairColor}"
                    + $"{Environment.NewLine}\tEye Color: {EyeColor}"
                    + $"{Environment.NewLine}\tPassport Id: {PassportId}"
                    + $"{Environment.NewLine}\tCountry Id: {CountryId}";
            }

            private bool IsHeightValid
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(Height))
                        return false;

                    var pattern = new Regex(@"\b(?<height>\d+)(?<unit>in|cm)\b", RegexOptions.IgnoreCase);
                    var match = pattern.Match(Height);

                    if (match.Success)
                    {
                        var height = Convert.ToInt32(match.Groups["height"].Value);
                        var unit = match.Groups["unit"].Value;

                        if (unit.Equals("in", StringComparison.InvariantCultureIgnoreCase))
                            return height.Includes(59, 76);
                        if (unit.Equals("cm", StringComparison.InvariantCultureIgnoreCase))
                            return height.Includes(150, 193);
                    }

                    return false;
                }
            }

            private bool IsHairColorValid
            {
                get
                {
                    var pattern = new Regex(@"\#[0-9a-f]{6}");
                    var match = pattern.Match(HairColor);

                    return match.Success;
                }
            }

            private bool IsPassportIdValid
            {
                get
                {
                    var pattern = new Regex(@"\b[0-9]{9}\b");
                    var match = pattern.Match(PassportId);

                    return match.Success;
                }
            }

            public static Passport Parse(string rawData, bool useExtendedValidation)
            {
                var passport = new Passport();
                passport.UseExtendedValidation = useExtendedValidation;

                foreach (var token in rawData.Split())
                {
                    if (string.IsNullOrWhiteSpace(token))
                        continue;

                    var kvp = token.ToKeyValuePair(':');
                    switch (kvp.Key)
                    {
                        case "byr":
                            passport.BirthYear = Convert.ToInt32(kvp.Value);
                            break;
                        case "iyr":
                            passport.IssueYear = Convert.ToInt32(kvp.Value);
                            break;
                        case "eyr":
                            passport.ExpirationYear = Convert.ToInt32(kvp.Value);
                            break;
                        case "hgt":
                            passport.Height = kvp.Value;
                            break;
                        case "hcl":
                            passport.HairColor = kvp.Value;
                            break;
                        case "ecl":
                            passport.EyeColor = EnumExtensions.ParseOrDefault<EyeColor>(kvp.Value, useExtendedValidation ? EyeColor.Unspecified : EyeColor.oth);
                            break;
                        case "pid":
                            passport.PassportId = kvp.Value;
                            break;
                        case "cid":
                            passport.CountryId = Convert.ToInt32(kvp.Value);
                            break;
                        default:
                            break;
                    }
                }

                return passport;
            }
        }

        public enum EyeColor
        {
            Unspecified,
            amb,
            blu,
            brn,
            gry,
            grn,
            hzl,
            oth
        }
    }
}
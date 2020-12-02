using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day1
    {
        const int _sum = 2020;

        private static IEnumerable<int> inputs
        {
            get
            {
                yield return 1446;
                yield return 1893;
                yield return 1827;
                yield return 1565;
                yield return 1728;
                yield return 497;
                yield return 1406;
                yield return 1960;
                yield return 1986;
                yield return 1945;
                yield return 1731;
                yield return 1925;
                yield return 1550;
                yield return 1841;
                yield return 1789;
                yield return 1952;
                yield return 1610;
                yield return 1601;
                yield return 1776;
                yield return 1808;
                yield return 1812;
                yield return 1834;
                yield return 1454;
                yield return 1729;
                yield return 513;
                yield return 1894;
                yield return 1703;
                yield return 1587;
                yield return 1788;
                yield return 1690;
                yield return 1655;
                yield return 1473;
                yield return 1822;
                yield return 1437;
                yield return 1626;
                yield return 1447;
                yield return 1400;
                yield return 1396;
                yield return 1715;
                yield return 1720;
                yield return 1469;
                yield return 1388;
                yield return 1874;
                yield return 1641;
                yield return 518;
                yield return 1664;
                yield return 1552;
                yield return 1800;
                yield return 512;
                yield return 1506;
                yield return 1806;
                yield return 1857;
                yield return 1802;
                yield return 1843;
                yield return 1956;
                yield return 1678;
                yield return 1560;
                yield return 1971;
                yield return 1940;
                yield return 1847;
                yield return 1902;
                yield return 1500;
                yield return 1383;
                yield return 1386;
                yield return 1398;
                yield return 1535;
                yield return 1713;
                yield return 1931;
                yield return 1619;
                yield return 1519;
                yield return 1897;
                yield return 1767;
                yield return 1548;
                yield return 1976;
                yield return 1984;
                yield return 1426;
                yield return 914;
                yield return 2000;
                yield return 1585;
                yield return 1634;
                yield return 1832;
                yield return 1849;
                yield return 1665;
                yield return 1609;
                yield return 1670;
                yield return 1520;
                yield return 1490;
                yield return 1746;
                yield return 1608;
                yield return 1829;
                yield return 1431;
                yield return 1762;
                yield return 1384;
                yield return 1504;
                yield return 1434;
                yield return 1356;
                yield return 1654;
                yield return 1719;
                yield return 1599;
                yield return 1686;
                yield return 1489;
                yield return 1377;
                yield return 1531;
                yield return 1912;
                yield return 144;
                yield return 1875;
                yield return 1532;
                yield return 1439;
                yield return 1482;
                yield return 1420;
                yield return 1529;
                yield return 1554;
                yield return 1826;
                yield return 1546;
                yield return 1589;
                yield return 1993;
                yield return 1518;
                yield return 1708;
                yield return 1733;
                yield return 1876;
                yield return 1953;
                yield return 1741;
                yield return 1689;
                yield return 773;
                yield return 1455;
                yield return 1613;
                yield return 2004;
                yield return 1819;
                yield return 1725;
                yield return 1617;
                yield return 1498;
                yield return 1651;
                yield return 2007;
                yield return 1402;
                yield return 728;
                yield return 1475;
                yield return 1928;
                yield return 1904;
                yield return 1969;
                yield return 1851;
                yield return 1296;
                yield return 1558;
                yield return 1817;
                yield return 1663;
                yield return 1750;
                yield return 1780;
                yield return 1501;
                yield return 1443;
                yield return 1734;
                yield return 1977;
                yield return 1901;
                yield return 1547;
                yield return 1631;
                yield return 1644;
                yield return 1815;
                yield return 1949;
                yield return 1586;
                yield return 1697;
                yield return 1435;
                yield return 1783;
                yield return 1772;
                yield return 1987;
                yield return 1483;
                yield return 1372;
                yield return 1999;
                yield return 1848;
                yield return 1512;
                yield return 1541;
                yield return 1861;
                yield return 2008;
                yield return 1607;
                yield return 1622;
                yield return 1629;
                yield return 1763;
                yield return 1656;
                yield return 1661;
                yield return 1581;
                yield return 1968;
                yield return 1985;
                yield return 1974;
                yield return 1882;
                yield return 995;
                yield return 1704;
                yield return 1896;
                yield return 1611;
                yield return 1888;
                yield return 1773;
                yield return 1810;
                yield return 1650;
                yield return 1712;
                yield return 1410;
                yield return 1796;
                yield return 1691;
                yield return 1671;
                yield return 1947;
                yield return 1775;
                yield return 1593;
                yield return 656;
                yield return 1530;
                yield return 1743;
            }
        }

        public static int Problem1()
        {
            var inputList = inputs.ToList();
            for (var i = 0; i < inputList.Count; i++)
                for (var j = i + 1; j < inputList.Count; j++)
                {
                    if (inputList[i] + inputList[j] == _sum)
                    {
                        var answer = inputList[i] * inputList[j];

                        Console.WriteLine($"Number 1: {inputList[i]}; Number 2: {inputList[j]}");
                        Console.WriteLine($"Answer: {answer}");

                        return answer;
                    }
                }

            Console.WriteLine($"Error: no 2 inputs sum to {_sum}...");
            return -1;
        }

        public static int Problem2()
        {
            var inputList = inputs.ToList();
            for (var i = 0; i < inputList.Count; i++)
                for (var j = i + 1; j < inputList.Count; j++)
                    for (var k = j + 1; k < inputList.Count; k++)
                    {
                        if (inputList[i] + inputList[j] + inputList[k] == _sum)
                        {
                            var answer = inputList[i] * inputList[j] * inputList[k];

                            Console.WriteLine($"Number 1: {inputList[i]}; Number 2: {inputList[j]}; Number 3: {inputList[k]}");
                            Console.WriteLine($"Answer: {answer}");

                            return answer;
                        }
                    }

            Console.WriteLine($"Error: no 3 inputs sum to {_sum}...");
            return -1;
        }
    }
}

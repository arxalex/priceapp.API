<?php

namespace framework\database;

class StringHelper
{
    public static function quotesReplacer(string $str): string
    {
        return str_replace(
            array('®', '™', '©', '‘', '’', '‚', '‛', '❛', '❜', '＇', '«', '»', '‹', '›', '“', '”', '„', '‟', '❝', '❞', '〝', '〞', '〟', '＂'),
            array('', '', '', '\'', '\'', '\'', '\'', '\'', '\'', '\'', '"', '"', '"', '"', '"', '"', '"', '"', '"', '"', '"', '"', '"', '"'),
            $str
        );
    }
    public static function getLabelsInsideQoutes(string $str): array
    {
        preg_match('/".*?"/', $str, $matches);
        return $matches;
    }
    public static function nameToKeywords(string $str): array
    {
        $nameStr = StringHelper::quotesReplacer($str);
        $nameStrWoQ = str_replace('"', '', $nameStr);
        return explode(' ', $nameStrWoQ);
    }
    public static function stringCleaner($str): string
    {
        return preg_replace('~[^\p{Cyrillic}a-z0-9_\s-]+~ui', '', $str);
    }
    public static function rateItemsByKeywords(string $label, array $items) : array
    {
        $labelArr = self::nameToKeywords(self::stringCleaner($label));
        $rates = [];
        foreach ($items as $key => $item) {
            $itemStrArr = self::nameToKeywords(self::stringCleaner($item));
            $tempRate = 0;
            $tempRate += $item == $label ? count($labelArr) * 8 : 0;
            $tempRate += self::stringContains($item, $label) ? count($labelArr) * 4 : 0;
            $tempRate += self::stringContains($label, $item) ? count($itemStrArr) * 4 : 0;
            $i = 0;
            $maxI = count($labelArr);
            foreach ($itemStrArr as $itemStr) {
                if ($itemStr == $labelArr[$i]) {
                    $tempRate += 2;
                    $i++;
                    if ($i == $maxI) {
                        break;
                    }
                } else {
                    continue;
                }
            }
            $i = 0;
            $maxI = count($itemStrArr);
            foreach ($labelArr as $labelStr) {
                if ($labelStr == $itemStrArr[$i]) {
                    $tempRate += 2;
                    $i++;
                    if ($i == $maxI) {
                        break;
                    }
                } else {
                    continue;
                }
            }
            foreach ($itemStrArr as $itemStr) {
                foreach ($labelArr as $labelStr) {
                    $tempRate += $labelStr == $itemStr ? 1 : 0;
                }
            }
            $rates[$key] = $tempRate;
        }
        return $rates;
    }
    public static function stringContains($haystack, $needle){
        return strpos($haystack, $needle) === false ? false : true;
    }
}

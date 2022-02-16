<?php

namespace framework\database;

class SqlHelper
{
    static function unicodeString($str, $encoding = null)
    {
        if (is_null($encoding)) $encoding = ini_get('mbstring.internal_encoding');
        return preg_replace_callback('/\\\\u([0-9a-fA-F]{4})/u', function ($match) use ($encoding) {
            return mb_convert_encoding(pack('H*', $match[1]), $encoding, 'UTF-16BE');
        }, $str);
    }
    static function mysql_escape_mimic($inp)
    {
        if (is_array($inp))
            return array_map(__METHOD__, $inp);

        if (!empty($inp) && is_string($inp)) {
            return str_replace(array('\\', "\0", "\n", "\r", "'", '"', "\x1a"), array('\\\\', '\\0', '\\n', '\\r', "\\'", '\\"', '\\Z'), $inp);
        }

        return $inp;
    }
    /**
     * Return query string that goes after "VALUES "
     */
    public static function insertObjects(array $objects){
        $query = "";
        
        foreach($objects as $obj){
            $query .= "(";
            foreach($obj as $key => $value)
            {
                $v = "DEFAULT";
                if(is_array($value)){
                    $v = "'". json_encode($value) ."'";
                } elseif (is_object($value)){
                    $v = "'". json_encode($value) ."'";
                } elseif ($value == null) {
                    $v = "DEFAULT";
                } else {
                    $v = "'". $value ."'";
                }
                $query .= "$key = $value, ";
            }
            
            $query = substr($query, 0, -2). "), ";
        }
        $query = substr($query, 0, -2);
    }

    /**
     * Return query string that implements ($value[0], ...) for "in"
     */
    public static function arrayInNumeric(array $values){
        $result = "(";
        foreach($values as $value){
            $result .= "$value, ";
        }
        $result = substr($result, 0, -2). ")";
        return $result;
    }

    /**
     * Return query string that implements ($value[0], ...) for "in"
     */
    public static function arrayInString(array $values){
        $result = "(";
        foreach($values as $value){
            $value = self::mysql_escape_mimic($value);
            $result .= "'$value', ";
        }
        $result = substr($result, 0, -2). ")";
        return $result;
    }

    /**
     * Return query string that implements ($value[0], ...) for "in"
     */
    public static function arrayLikeString(string $key, array $values){
        $result = "(";
        foreach($values as $value){
            $value = self::mysql_escape_mimic($value);
            $result .= "$key LIKE '%$value%' OR ";
        }
        $result = substr($result, 0, -4). ")";
        return $result;
    }

    /**
     * Return query string that goes after "WHERE "
     */
    public static function whereCreate(array $where){
        $query = "";
        foreach($where as $key => $value){
            echo $key;
            if(is_numeric($value[0])){
                $query .= "$key in ";
                $query .= self::arrayInNumeric($value);
                $query .= " AND ";
            } elseif (is_string($value[0]) && substr($key, -5, 0) == "_like"){
                $key = substr($key, 0, -5);
                $query .= self::arrayLikeString($key, $value);
                $query .= " AND ";
            } elseif (is_string($value[0]) && substr($key, -5, 0) != "_like"){
                $query .= "$key in ";
                $query .= self::arrayInString($value);
                $query .= " AND ";
            }
        }
        $query = substr($query, 0, -5);
        return $query;
    }
}

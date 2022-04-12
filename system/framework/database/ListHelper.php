<?php

namespace framework\database;

use stdClass;

class ListHelper
{
    public static function deleteLowerThen(array $numbers, float $limit, bool $equalOk = true) : array
    {
        $result = [];
        if($equalOk){
            foreach($numbers as $number){
                if($number >= $limit){
                    $result[] = $number;
                }
            }
        } else {
            foreach($numbers as $number){
                if($number > $limit){
                    $result[] = $number;
                }
            }
        }
        return $result;

    }
    public static function getMin(array $numbers) : float
    {
        if(count($numbers) < 1){
            return false;
        }
        $max = $numbers[0];
        foreach($numbers as $number){
            $max = $max < $number ? $max : $number;
        }
        return $max;
    }
    public static function getMax(array $numbers) : float
    {
        if(count($numbers) < 1){
            return;
        }
        $max = $numbers[0];
        foreach($numbers as $number){
            $max = $max > $number ? $max : $number;
        }
        return $max;
    }
    public static function getColumn(array $objects, string $key) : array
    {
        $result = [];
        foreach($objects as $object){
            $result[] = $object->$key;
        }
        return $result;
    }
    public static function getColumns(array $objects, array $keys) : array
    {
        $result = [];
        foreach($objects as $object){
            $temp = new stdClass();
            foreach($keys as $key){
                $temp->$key = $object->$key;
            }
            $result[] = $temp;
        }
        return $result;
    }
    public static function isObjectinArray(object $needle, array $haystack, array $fieldNames) : bool
    {
        $result = false;

        foreach($haystack as $value){
            $preResult = true;
            foreach($fieldNames as $field){
                if($value->$field != $needle->$field){
                    $preResult = false;
                    break;
                }
            }
            
            if($preResult == true){
                $result = true;
                break;
            }
        }

        return $result;
    }
    public static function numberObjectinArray(object $needle, array $haystack, array $fieldNames) : int
    {
        $result = -1;

        foreach($haystack as $key => $value){
            $preResult = true;
            foreach($fieldNames as $field){
                if($value->$field != $needle->$field){
                    $preResult = false;
                    break;
                }
            }
            
            if($preResult == true){
                $result = $key;
                break;
            }
        }

        return $result;
    }
    public static function getMultipleByFields(array $haystack, array $fields) : array
    {
        $result = [];

        foreach($haystack as $value){
            $preResult = true;
            foreach($fields as $key => $field){
                if(!in_array($value->$key, $field)){
                    $preResult = false;
                    break;
                }
            }
            
            if($preResult == true){
                $result[] = $value;
            }
        }

        return $result;
    }

    public static function getOneByFields(array $haystack, array $fields) : ?object
    {
        $result = null;

        foreach($haystack as $value){
            $preResult = true;
            foreach($fields as $key => $field){
                if($value->$key != $field){
                    $preResult = false;
                    break;
                }
            }
            
            if($preResult == true){
                $result = $value;
                break;
            }
        }

        return $result;
    }
} 
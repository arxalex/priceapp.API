<?php

namespace framework\database;

use stdClass;

class ListHelper
{
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
} 
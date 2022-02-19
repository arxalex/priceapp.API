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
}
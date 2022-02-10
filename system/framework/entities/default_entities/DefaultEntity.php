<?php

namespace framework\entities\default_entities;

class DefaultEntity
{
    public ?int $id;

    public function __construct(
        ?int $id = null
    ) {
        $this->id = $id;
    }
    public function stringConvert($value){
        if(is_numeric($value)){
            return $value;
        } elseif(is_string($value)) {
            if(is_object(json_decode($value))) {
                return json_decode($value);
            } elseif (is_array(json_decode($value))){
                return json_decode($value);
            }
        } elseif(is_array($value)){
            return $value;
        } elseif(is_object($value)){
            return $value;
        }
    }
    public static function isJson(?string $string){
        return ((is_string($string) && (is_object(json_decode($string)) || is_array(json_decode($string))))) ? true : false;
    }
}

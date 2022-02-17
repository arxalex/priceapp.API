<?php

namespace framework\database;

use framework\core\constants\ConstantsService;
use framework\entities\default_entities\DefaultEntity;
use PDO;

class Request
{
    private ConstantsService $_constantsService;
    private PDO $connection;
    private $statement;


    public function __construct(string $query)
    {
        $this->_constantsService = new ConstantsService();
        $this->connection = new PDO(
            "mysql:host=" . $this->_constantsService->getConstant('DB_host') . "; dbname=" . $this->_constantsService->getConstant('DB_dbname') . "; charset=utf8",
            $this->_constantsService->getConstant('DB_username'),
            $this->_constantsService->getConstant('DB_password')
        );
        $this->statement = $this->connection->prepare($query);
    }
    public function execute()
    {
        $this->statement->execute();
    }
    public function fetch($method = PDO::FETCH_ASSOC){
        return $this->statement->fetch($method);
    }
    public function fetchAll($method = PDO::FETCH_CLASS, ?string $class = "stdClass"){
        if($method == PDO::FETCH_CLASS){
            $response = $this->statement->fetchAll(PDO::FETCH_ASSOC);
            $result = [];
            foreach($response as $valueR){
                $preResult = new $class();
                foreach($valueR as $key => $value){
                    if(DefaultEntity::isJson($value)){
                        $preResult->$key = json_decode($value);
                    } else {
                        $preResult->$key = $value;
                    }
                }
                $result[] = $preResult;
            } 
            return $result;
        } else {
            return $this->statement->fetchAll($method);
        }
    }
    public function fetchObject(?string $class = "stdClass", array $constructorArgs = []){
        $response = $this->fetch();
        $result = new $class();
        foreach($response as $key => $value){
            if(DefaultEntity::isJson($value)){
                $result->$key = json_decode($value);
            } else {
                $result->$key = $value;
            }
        }

        return $result;
    }
}

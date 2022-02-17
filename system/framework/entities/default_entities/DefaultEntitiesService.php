<?php

namespace framework\entities\default_entities;

use framework\database\Request;
use framework\database\SqlHelper;
use PDO;

class DefaultEntitiesService
{
    protected const ENTITIES_NAMESPACE = "framework\\entities\\";
    protected string $className;
    protected string $tableName;

    public function __construct()
    {
    }
    public function getItemFromDB(int $id)
    {
        $table = $this->tableName;
        $query = "select * from `$table` where id = $id";
        $connection = new Request($query);
        $connection->execute();
        $response = $connection->fetchObject($this->className);
        return $response;
    }
    public function getItemsFromDB(array $where = []): array
    {
        $table = $this->tableName;
        if (count($where) != 0) {
            $whereQuery = SqlHelper::whereCreate($where);
            $query = "select * from `$table` where $whereQuery";
        } else {
            $query = "select * from `$table`";
        }
        $connection = new Request($query);
        $connection->execute();
        $response = $connection->fetchAll(PDO::FETCH_CLASS, $this->className);
        return $response;
    }
    public function insertItemToDB($item)
    {
        $table = $this->tableName;
        $query = "insert into `$table`
        values " . SqlHelper::insertObjects([$item]);
        return (new Request($query))->execute();
    }
    public function orderItemsByRate(array $items, array $rates, int $max = null) : array
    {
        $count = count($rates);
        if($count <= 1){
            return $items;
        }

        for($i = 1; $i < $count; $i++) {
            $tempRate = $rates[$i];
            $tempItem = $items[$i];
            $j = $i - 1;

            while(isset($rates[$j]) && $rates[$j] < $tempRate){
                $rates[$j + 1] = $rates[$j];
                $rates[$j] = $tempRate;
                $items[$j + 1] = $items[$j];
                $items[$j] = $tempItem;
                $j--;
            }
        }
        if($max === null){
            return $items;
        }
        $result = [];
        for($i = 0; $i < $max; $i++) {
            $result[] = $items[$i];
        }
        return $result;
    }
    public function getColumn(array $objects, string $key) : array
    {
        $result = [];
        foreach($objects as $object){
            $result[] = $object->$key;
        }
        return $result;
    }
}

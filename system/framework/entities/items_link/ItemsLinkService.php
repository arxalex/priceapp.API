<?php

namespace framework\entities\items_link;

use framework\database\Request;
use framework\entities\items_link\ItemLink;
use framework\database\SqlHelper;

class ItemsLinkService
{
    public function __construct()
    {
    }
    public function getItemLinkFromDB(int $id) {
        $query = "select top 1 * from pa_items_link where id = $id";
        $response = (new Request($query))->fetchObject("ItemLink");
    }
    public function insertItemLinkToDB(ItemLink $itemLink){
        $query = "insert into pa_items_link
        values " . SqlHelper::insertObjects([$itemLink]);
        $response = (new Request($query))->execute();
    }
}

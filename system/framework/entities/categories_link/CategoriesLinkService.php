<?php

namespace framework\entities\categories_link;

use framework\database\Request;
use framework\entities\categories_link\CategoryLink;
use framework\database\SqlHelper;

class CategoriesLinkService
{
    public function __construct()
    {
    }
    public function getCategoryLinkFromDB(int $id) {
        $query = "select top 1 * from pa_categories_link where id = $id";
        $response = (new Request($query))->fetchObject("CategoryLink");
    }
    public function insertCategoryLinkToDB(CategoryLink $categoryLink){
        $query = "insert into pa_categories_link
        values " . SqlHelper::insertObjects([$categoryLink]);
        $response = (new Request($query))->execute();
    }
}

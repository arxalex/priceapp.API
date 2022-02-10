<?php

namespace framework\entities\categories;

use framework\database\Request;
use framework\entities\categories\Category;
use framework\database\SqlHelper;

class CategoriesService
{
    public function __construct()
    {
    }
    public function getCategoryFromDB(int $id) {
        $query = "select top 1 * from pa_categories where id = $id";
        $response = (new Request($query))->fetchObject("Category");
    }
    public function insertCategoryToDB(Category $category){
        $query = "insert into pa_categories
        values " . SqlHelper::insertObjects([$category]);
        $response = (new Request($query))->execute();
    }
}

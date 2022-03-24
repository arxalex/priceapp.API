<?php

namespace endpoint\categories;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\categories\CategoriesService;
use framework\entities\categories\Category;
use framework\entities\categories_link\CategoriesLinkService;

use stdClass;

class InsertCategories extends BaseEndpointBuilder
{
    public CategoriesLinkService $_categoriesLinkService;
    public CategoriesService $_categoriesService;
    public function __construct()
    {
        parent::__construct();
        $this->_categoriesLinkService = new CategoriesLinkService();
        $this->_categoriesService = new CategoriesService();
    }
    public function defaultParams()
    {
        return [
            'category' => null,
            'category_link' => null,
            'method' => "InsertToCategoriesAndUpdateLink",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if($this->getParam('method') == "InsertToCategoriesAndUpdateLink"){
            $categoryModel = (object) $this->getParam('category');
            $categoryLinkModel = (object) $this->getParam('category_link');

            $category = new Category(null, $categoryModel->label, $categoryModel->parent, $categoryModel->isFilter);
            $categoryLink = $this->_categoriesLinkService->getItemFromDB($categoryLinkModel->id);
            
            $this->_categoriesService->insertItemToDB($category);
            $category = $this->_categoriesService->getLastInsertedItem();
            $categoryLink->categoryid = $category->id;
            $this->_categoriesLinkService->updateItemInDB($categoryLink);

            $statusInsert = $this->_categoriesService->getItemFromDB($category->id)->label == $categoryModel->label;
            $statusUpdate = $this->_categoriesLinkService->getItemFromDB($categoryLink->id)->categoryid == $categoryLink->categoryid;
            $result = new stdClass();
            $result->statusUpdate = $statusUpdate;
            $result->statusInsert = $statusInsert;
            return $result;
        } elseif($this->getParam('method') == "InsertToCategories"){
            $categoryModel = (object) $this->getParam('category');

            $category = new Category(null, $categoryModel->label, $categoryModel->parent, $categoryModel->isFilter);
            
            $this->_categoriesService->insertItemToDB($category);
            $category = $this->_categoriesService->getLastInsertedItem();

            $statusInsert = $this->_categoriesService->getItemFromDB($category->id)->label == $categoryModel->label;
            $result = new stdClass();
            $result->statusInsert = $statusInsert;
            $result->category = $category;
            return $result;
        } elseif($this->getParam('method') == "InsertOrUpdateLink"){
            $categoryLinkModel = (object) $this->getParam('category_link');
            $result = new stdClass();

            if($categoryLinkModel->id == null || $categoryLinkModel->id == ""){
                $this->_categoriesLinkService->insertItemToDB($categoryLinkModel);
                $categoryLink = $this->_categoriesLinkService->getLastInsertedItem();
                $statusInsert = $this->_categoriesLinkService->getItemFromDB($categoryLink->id)->shopcategorylabel == $categoryLinkModel->shopcategorylabel;
                $result->statusInsert = $statusInsert;
                $result->insertedItem = $categoryLink; 
            } else {
                $categoryLink = $this->_categoriesLinkService->getItemFromDB($categoryLinkModel->id);
                $this->_categoriesLinkService->updateItemInDB($categoryLink);
                $statusUpdate = $this->_categoriesLinkService->getItemFromDB($categoryLink->id)->categoryid == $categoryLink->categoryid;
                $result->statusUpdate = $statusUpdate;
            }
            return $result;
        }
    }
}

<?php

namespace endpoint\categories;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\categories\CategoriesService;
use framework\entities\categories\Category;
use framework\entities\categories_link\CategoriesLinkService;
use framework\entities\categories_link\CategoryLink;

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
            'method' => "InsertToCategoriesAndUpdateLink"
        ];
    }
    public function build()
    {
        if($this->getParam('method') == "InsertToCategoriesAndUpdateLink"){
            $categoryModel = $this->getParam('category');
            $categoryLinkModel = $this->getParam('category_link');
            
            $category = new Category(null, $categoryModel->label, $categoryModel->parent, $categoryModel->isFilter);
            $categoryLink = $this->_categoriesLinkService->getItemFromDB($categoryLinkModel->id);
            
            $this->_categoriesService->insertItemToDB($category);
            $category = $this->_categoriesService->getLastInsertedItem();
            $categoryLink->categoryid = $category->id;
            $this->_categoriesLinkService->updateItemInDB($categoryLink);
        }
    }
}

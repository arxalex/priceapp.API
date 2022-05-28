<?php

namespace endpoint\categories;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\categories_link\CategoriesLinkService;
use framework\entities\categories_link\CategoryLink;
use framework\shops\silpo\SilpoCategoriesGetter;
use framework\shops\fora\ForaCategoriesGetter;
use stdClass;

class GetCategoriesFromShopAndInsertToDB extends BaseEndpointBuilder
{
    private SilpoCategoriesGetter $_silpoCategoriesGetter;
    private CategoriesLinkService $_categoriesLinkService;
    private ForaCategoriesGetter $_foraCategoriesGetter;

    public function __construct()
    {
        parent::__construct();
        $this->_silpoCategoriesGetter = new SilpoCategoriesGetter();
        $this->_foraCategoriesGetter = new ForaCategoriesGetter();
        $this->_categoriesLinkService = new CategoriesLinkService();
    }
    public function defaultParams()
    {
        return [
            'source' => 0,
            'method' => "GetCategoryFromSourceAndInsert",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 9);
        $result = new stdClass();
        if ($this->getParam('method') == "GetCategoryFromSourceAndInsert") {
            if ($this->getParam('source') === 1) {
                $i = 0;
                $categories = $this->_silpoCategoriesGetter->get();
                foreach($categories as $category){
                    if(count($this->_categoriesLinkService->getItemsFromDB(['shopid' => [1], 'shopcategorylabel' => [$category->label]])) <= 0){
                        $otherCategoryLinks = $this->_categoriesLinkService->getItemsFromDB(['shopcategorylabel' => [$category->label]]);
                        $categoryId = 0;
                        if(count($otherCategoryLinks) == 1){
                            $categoryId = ($otherCategoryLinks[0])->categoryid;
                        }
                        $this->_categoriesLinkService->insertItemToDB(new CategoryLink(null, $categoryId, 1, $category->id, $category->label));
                        $i++;
                    }
                }
                $result->count = $i;
                $result->statusInsert = true;
                return $result;
            } elseif ($this->getParam('source') === 2) {
                $i = 0;
                $categories = $this->_foraCategoriesGetter->get();
                foreach($categories as $category){
                    if(count($this->_categoriesLinkService->getItemsFromDB(['shopid' => [2], 'shopcategorylabel' => [$category->label]])) <= 0){
                        $otherCategoryLinks = $this->_categoriesLinkService->getItemsFromDB(['shopcategorylabel' => [$category->label]]);
                        $categoryId = 0;
                        if(count($otherCategoryLinks) == 1){
                            $categoryId = ($otherCategoryLinks[0])->categoryid;
                        }
                        $this->_categoriesLinkService->insertItemToDB(new CategoryLink(null, $categoryId, 2, $category->id, $category->label));
                        $i++;
                    }
                }
                $result->count = $i;
                $result->statusInsert = true;
                return $result;
            }
        } 
    }
}

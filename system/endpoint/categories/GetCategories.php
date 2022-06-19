<?php

namespace endpoint\categories;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\categories\CategoriesService;
use framework\entities\categories_link\CategoriesLinkService;
use framework\shops\atb\AtbCategoriesGetter;
use framework\shops\silpo\SilpoCategoriesGetter;
use framework\shops\fora\ForaCategoriesGetter;

class GetCategories extends BaseEndpointBuilder
{
    private SilpoCategoriesGetter $_silpoCategoriesGetter;
    private CategoriesLinkService $_categoriesLinkService;
    private CategoriesService $_categoriesService;
    private AtbCategoriesGetter $_atbCategoriesGetter;
    private ForaCategoriesGetter $_foraCategoriesGetter;

    public function __construct()
    {
        parent::__construct();
        $this->_silpoCategoriesGetter = new SilpoCategoriesGetter();
        $this->_categoriesLinkService = new CategoriesLinkService();
        $this->_categoriesService = new CategoriesService();
        $this->_atbCategoriesGetter = new AtbCategoriesGetter();
        $this->_foraCategoriesGetter = new ForaCategoriesGetter();
    }
    public function defaultParams()
    {
        return [
            'source' => 0,
            'method' => "GetCategoryFromSource",
            'label' => "",
            'parent' => NULL,
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if ($this->getParam('method') == "GetCategoryFromSource") {
            if ($this->getParam('source') === 0) {
                return $this->_categoriesService->getItemsFromDB(["parent" => [$this->getParam('parent')]]);
            } elseif ($this->getParam('source') === 1) {
                $this->_usersService->unavaliableRequest($this->getParam('cookie'), 9);
                return $this->_silpoCategoriesGetter->get();
            } elseif ($this->getParam('source') === 2) {
                $this->_usersService->unavaliableRequest($this->getParam('cookie'), 9);
                return $this->_foraCategoriesGetter->get();
            } elseif ($this->getParam('source') === 3) {
                $this->_usersService->unavaliableRequest($this->getParam('cookie'), 9);
                return $this->_atbCategoriesGetter->get();
            }
        } elseif ($this->getParam('method') == "GetCategoryLinkByLabel") {
            $this->_usersService->unavaliableRequest($this->getParam('cookie'), 9);
            $label = $this->getParam('label');
            return $this->_categoriesLinkService->getCategoryLink($label);
        }
    }
}

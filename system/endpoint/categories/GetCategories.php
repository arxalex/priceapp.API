<?php

namespace endpoint\categories;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\categories_link\CategoriesLinkService;
use framework\shops\silpo\SilpoCategoriesGetter;

class GetCategories extends BaseEndpointBuilder
{
    private SilpoCategoriesGetter $_silpoCategoriesGetter;
    private CategoriesLinkService $_categoriesLinkService;

    public function __construct()
    {
        parent::__construct();
        $this->_silpoCategoriesGetter = new SilpoCategoriesGetter();
        $this->_categoriesLinkService = new CategoriesLinkService();
    }
    public function defaultParams()
    {
        return [
            'source' => 0,
            'method' => "GetCategoryFromSource",
            'label' => "",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if ($this->getParam('method') == "GetCategoryFromSource") {
            if ($this->getParam('source') === 0) {
                return [];
            } elseif ($this->getParam('source') === 1) {
                return $this->_silpoCategoriesGetter->get();
            }
        } elseif ($this->getParam('method') == "GetCategoryLinkByLabel") {
            $label = $this->getParam('label');
            return $this->_categoriesLinkService->getCategoryLink($label);
        }
    }
}

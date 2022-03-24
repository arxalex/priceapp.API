<?php

namespace endpoint\brands;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\brands\BrandsService;

class GetBrands extends BaseEndpointBuilder
{
    private BrandsService $_brandsService;
    public function __construct()
    {
        parent::__construct(); 
        $this->_brandsService = new BrandsService();
    }
    public function defaultParams()
    {
        return [
            'method' => "GetBrandByLabel",
            'label' => "",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if ($this->getParam('method') == "GetBrandByLabel") {
            $label = $this->getParam('label');
            return $this->_brandsService->getBrand($label);
        }
    }
}

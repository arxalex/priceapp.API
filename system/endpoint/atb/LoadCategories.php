<?php

namespace endpoint\atb;

use atb\AtbCategories;
use endpoint\defaultBuild\BaseEndpointBuilder;

class LoadCategories extends BaseEndpointBuilder
{
    private AtbCategories $_atbCategories;
    public function __construct()
    {
        parent::__construct(); 
        $this->_atbCategories = new AtbCategories();
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        return $this->_atbCategories->loadCategories();
    }
}

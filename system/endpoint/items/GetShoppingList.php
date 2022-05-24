<?php

namespace endpoint\items;

use businessLogic\prices\PricesWebService;
use endpoint\defaultBuild\BaseEndpointBuilder;
use stdClass;

class GetShoppingList extends BaseEndpointBuilder
{
    private PricesWebService $_pricesWebService;
    public function __construct()
    {
        parent::__construct();
        $this->_pricesWebService = new PricesWebService();
    }
    public function defaultParams()
    {
        return [
            'method' => 'multipleLowest',
            'items' => [],
            'xCord' => null,
            'yCord' => null,
            'radius' => null,
            'cookie' => []
        ];
    }
    public function build()
    {
        $result = new stdClass();
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if (
            $this->getParam('xCord') != null
            && $this->getParam('yCord') != null
            && $this->getParam('radius') != null
        ) {
            if ($this->getParam('method') == "multipleLowest") {
                $result->shoppingList = $this->_pricesWebService->getShoppingListMultipleLowest(
                    $this->getParam('items'),
                    $this->getParam('xCord'),
                    $this->getParam('yCord'),
                    $this->getParam('radius')
                );
                $result->economy = 0;
                return $result;
            }
        }
    }
}

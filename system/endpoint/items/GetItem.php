<?php

namespace endpoint\items;

use businessLogic\items\ItemsWebService;
use businessLogic\prices\PricesWebService;
use endpoint\defaultBuild\BaseEndpointBuilder;

class GetItem extends BaseEndpointBuilder
{
    private ItemsWebService $_itemsWebService;
    private PricesWebService $_pricesWebService;
    public function __construct()
    {
        parent::__construct();
        $this->_itemsWebService = new ItemsWebService();
        $this->_pricesWebService = new PricesWebService();
    }
    public function defaultParams()
    {
        return [
            'id' => null,
            'method' => 'viewModelById',
            'source' => 0,
            'xCord' => null,
            'yCord' => null,
            'radius' => null,
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if ($this->getParam('source') === 0) {
            if ($this->getParam('method') == 'viewModelById' && $this->getParam('id') != null) {
                return $this->_itemsWebService->getItemViewModelById($this->getParam('id'));
            } elseif (
                $this->getParam('method') == 'pricesAndFilialsViewModelById'
                && $this->getParam('id') != null
                && $this->getParam('xCord') != null
                && $this->getParam('yCord') != null
                && $this->getParam('radius') != null
            ) {
                return $this->_pricesWebService->getPricesWithFilialsViewModelByItemIdAndCord(
                    $this->getParam('id'),
                    $this->getParam('xCord'),
                    $this->getParam('yCord'),
                    $this->getParam('radius')
                );
            } elseif (
                $this->getParam('method') == 'viewModelByIdAndLocation'
                && $this->getParam('id') != null
                && $this->getParam('xCord') != null
                && $this->getParam('yCord') != null
                && $this->getParam('radius') != null
            ) {
                return $this->_itemsWebService->getItemViewModelById(
                    $this->getParam('id'),
                    $this->getParam('xCord'),
                    $this->getParam('yCord'),
                    $this->getParam('radius')
                );
            }
        }
    }
}

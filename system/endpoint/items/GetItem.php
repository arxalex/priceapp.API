<?php

namespace endpoint\items;

use businessLogic\items\ItemsWebService;
use endpoint\defaultBuild\BaseEndpointBuilder;

class GetItem extends BaseEndpointBuilder
{
    private ItemsWebService $_itemsWebService;
    public function __construct()
    {
        parent::__construct();
        $this->_itemsWebService = new ItemsWebService();
    }
    public function defaultParams()
    {
        return [
            'id' => null,
            'method' => 'viewModelById',
            'source' => 0,
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if ($this->getParam('source') === 0) {
            if ($this->getParam('method') == 'viewModelById' && $this->getParam('id') != null) {
                $result = $this->_itemsWebService->getItemViewModelById($this->getParam('id'));

                return $result;
            }
        }
    }
}

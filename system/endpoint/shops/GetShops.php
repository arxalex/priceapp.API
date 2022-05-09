<?php

namespace endpoint\shops;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\shops\ShopsService;

class GetShops extends BaseEndpointBuilder
{
    private ShopsService $_shopsService;
    public function __construct()
    {
        parent::__construct();
        $this->_shopsService = new ShopsService();
    }
    public function defaultParams()
    {
        return [
            'source' => 0,
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if ($this->getParam('source') === 0) {
            return $this->_shopsService->getItemsFromDB();
        }
    }
}

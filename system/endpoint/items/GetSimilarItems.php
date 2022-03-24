<?php

namespace endpoint\items;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\items\ItemsService;
use framework\database\StringHelper;
use framework\shops\silpo\SilpoItemsGetter;

class GetSimilarItems extends BaseEndpointBuilder
{
    private ItemsService $_itemsService;
    public function __construct()
    {
        parent::__construct();
        $this->_itemsService = new ItemsService();
    }
    public function defaultParams()
    {
        return [
            'itemLabels' => [],
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        $labels = $this->getParam('itemLabels');
        
        if(count($labels) < 1){
            return [];
        }
        $similarItems = [];
        foreach($labels as $label){
            $similarItems[] = $this->_itemsService->getSimilarItemsByLabel($label);
        }
        return $similarItems;
    }
}

<?php

namespace endpoint\items;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\items\ItemsService;
use framework\database\StringHelper;
use framework\shops\silpo\SilpoItemsGetter;

class GetItem extends BaseEndpointBuilder
{
    private ItemsService $_itemsService;
    private SilpoItemsGetter $_silpoItemsGetter;
    public function __construct()
    {
        parent::__construct();
        $this->_itemsService = new ItemsService();
        $this->_silpoItemsGetter = new SilpoItemsGetter();
    }
    public function defaultParams()
    {
        return [
            'id' => null,
            'method' => 'byId',
            'source' => 0,
            'name' => null,
            'category' => null
        ];
    }
    public function build()
    {
        if ($this->getParam('source') === 0) {
            if ($this->getParam('method') == 'byId') {
                return $this->_itemsService->getItemFromDB($this->getParam('id'));
            } elseif ($this->getParam('method') == 'byNameAndCategory') {
                $label = $this->getParam('name');
                $nameArr = StringHelper::nameToKeywords($this->getParam('name'));
                $result = $this->_itemsService->getItemsFromDB([
                    'label_like' => $nameArr,
                    'category' => [],
                ]);
                $rate = StringHelper::rateItemsByKeywords($label, array_column($result, 'label'));
                return $this->_itemsService->orderItemsByRate($result, $rate, 5);
            }
        } elseif($this->getParam('source') === 1){
            return $this->_silpoItemsGetter->get($this->getParam('category'));
            
        }
    }
}

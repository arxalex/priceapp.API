<?php

namespace endpoint\categories;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\shops\silpo\SilpoItemsGetter;

class GetCategories extends BaseEndpointBuilder
{
    private SilpoItemsGetter $_silpoItemsGetter;
    public function __construct()
    {
        parent::__construct();
        $this->_silpoItemsGetter = new SilpoItemsGetter();
    }
    public function defaultParams()
    {
        return [
            'source' => 0,
        ];
    }
    public function build()
    {
        if ($this->getParam('source') === 0) {
            return [];
        } elseif($this->getParam('source') === 1){
            $silpoItemsModels = $this->_silpoItemsGetter->get($this->getParam('category'));
            $items = [];
            foreach($silpoItemsModels as $silpoItemModel){
                $items[] = $this->_silpoItemsGetter->convertFromSilpoToCommonModel($silpoItemModel);
            }
            return $items;
        }
    }
}

<?php

namespace endpoint\categories;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\shops\silpo\SilpoCategoriesGetter;

class GetCategories extends BaseEndpointBuilder
{
    private SilpoCategoriesGetter $_silpoCategoriesGetter;
    public function __construct()
    {
        parent::__construct();
        $this->_silpoCategoriesGetter = new SilpoCategoriesGetter();
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
            return $this->_silpoCategoriesGetter->get();
        }
    }
}

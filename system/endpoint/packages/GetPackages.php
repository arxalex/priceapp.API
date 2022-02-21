<?php

namespace endpoint\packages;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\packages\PackagesService;

class GetCategories extends BaseEndpointBuilder
{
    private PackagesService $_packagesService;
    public function __construct()
    {
        parent::__construct(); 
        $this->_packagesService = new PackagesService();
    }
    public function defaultParams()
    {
        return [
            'method' => "GetCategoryFromSource",
            'label' => ""
        ];
    }
    public function build()
    {
        if ($this->getParam('method') == "GetPackageByLabel") {
            $label = $this->getParam('label');
            return $this->_packagesService->getPackage($label);
        }
    }
}

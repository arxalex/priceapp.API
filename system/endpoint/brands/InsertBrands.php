<?php

namespace endpoint\brands;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\brands\BrandsService;
use framework\entities\brands\Brand;
use stdClass;

class InsertBrands extends BaseEndpointBuilder
{
    private BrandsService $_brandsService;
    public function __construct()
    {
        $this->_brandsService = new BrandsService();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'brand' => null,
            'method' => "InsertOrUpdateBrand",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if ($this->getParam('method') == "InsertOrUpdateBrand") {
            $brandModel = (object) $this->getParam('brand');
            $result = new stdClass();
            if ($brandModel->id == null || $brandModel->id == "") {
                $brand = new Brand(null, $brandModel->label, $brandModel->short);
                $this->_brandsService->insertItemToDB($brand);
                $brand = $this->_brandsService->getLastInsertedItem();
                $statusInsert = $this->_brandsService->getItemFromDB($brand->id)->label == $brandModel->label;
                $result->statusInsert = $statusInsert;
                $result->brand = $brand;
            } else {
                $brand = new Brand(intval($brandModel->id), $brandModel->label, $brandModel->short);
                $this->_brandsService->updateItemInDB($brand);
                $statusUpdate = $this->_brandsService->getItemFromDB($brand->id)->label == $brand->label;
                $result->statusUpdate = $statusUpdate;
            }

            return $result;
        }
    }
}

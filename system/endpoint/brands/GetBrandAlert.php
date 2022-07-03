<?php

namespace endpoint\brands;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\brand_alerts\BrandAlertsService;

class GetBrandAlert extends BaseEndpointBuilder
{
    private BrandAlertsService $_brandAlertsService;
    public function __construct()
    {
        parent::__construct(); 
        $this->_brandAlertsService = new BrandAlertsService();
    }
    public function defaultParams()
    {
        return [
            'method' => "GetAlertByBrandId",
            'brandid' => "",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if ($this->getParam('method') == "GetAlertByBrandId") {
            $brandid = $this->getParam('brandid');
            return $this->_brandAlertsService->getItemsFromDB(['brandid' => [$brandid]]);
        }
    }
}

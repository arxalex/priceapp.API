<?php

namespace endpoint\consists;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\consists\ConsistsService;

class GetConsists extends BaseEndpointBuilder
{
    private ConsistsService $_consistsService;
    public function __construct()
    {
        parent::__construct(); 
        $this->_consistsService = new ConsistsService();
    }
    public function defaultParams()
    {
        return [
            'method' => "GetConsistByLabel",
            'label' => "",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if ($this->getParam('method') == "GetConsistByLabel") {
            $label = $this->getParam('label');
            return $this->_consistsService->getConsist($label);
        }
    }
}

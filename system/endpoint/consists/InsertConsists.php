<?php

namespace endpoint\consists;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\consists\ConsistsService;
use framework\entities\consists\Consist;
use stdClass;

class InsertConsists extends BaseEndpointBuilder
{
    private ConsistsService $_consistsService;
    public function __construct()
    {
        $this->_consistsService = new ConsistsService();
        parent::__construct();
    }
    public function defaultParams()
    {
        return [
            'consist' => null,
            'method' => "InsertOrUpdateConsist",
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        if ($this->getParam('method') == "InsertOrUpdateConsist") {
            $consistModel = (object) $this->getParam('consist');
            $result = new stdClass();
            if ($consistModel->id == null || $consistModel->id == "") {
                $consist = new Consist(null, $consistModel->label);
                $this->_consistsService->insertItemToDB($consist);
                $consist = $this->_consistsService->getLastInsertedItem();
                $statusInsert = $this->_consistsService->getItemFromDB($consist->id)->label == $consistModel->label;
                $result->statusInsert = $statusInsert;
                $result->consist = $consist;
            } else {
                $consist = new Consist(intval($consistModel->id), $consistModel->label);
                $this->_consistsService->updateItemInDB($consist);
                $statusUpdate = $this->_consistsService->getItemFromDB($consist->id)->label == $consist->label;
                $result->statusUpdate = $statusUpdate;
            }

            return $result;
        }
    }
}

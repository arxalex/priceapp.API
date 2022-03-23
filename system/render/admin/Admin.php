<?php

namespace render\admin;

use framework\entities\users\UsersService;
use render\defaultBuild\BaseRenderBuilder;

class Admin extends BaseRenderBuilder
{
    private UsersService $_usersService;
    public function __construct()
    {
        parent::__construct();
        $this->_usersService = new UsersService();
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        if (!empty($this->getParam('cookie'))) {
            $cookies = $this->getParam('cookie');
            if (isset($cookies['userid'])) {
                $user = $this->_usersService->getItemFromDB($cookies['userid']);
                if ($user->role == 9) {
                    return '
                    <div class="container">
                        <a class="btn btn-primary" href="/admin/items">Add items</a>
                    </div>';
                }
            }
        }
        header("Location: /login", true);
        die();
    }
}

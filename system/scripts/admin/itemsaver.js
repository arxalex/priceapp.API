Vue.component('itemsaver', {
    template: `
        <div>
            <categoryInsert 
                v-if="isCategoryInsertAwailible"
                @categoryInserted="categoryInserted"
                :sourceCategory="originalLabels.categoryLabel">
            </categoryInsert>
            <packageInsert 
                v-if="isPackageInsertAwailible"
                @packageInserted="packageInserted"
                :sourcePackage="originalLabels.packageLabel">
            </packageInsert>
            <table class="table word-break">
                <tbody>
                    <tr>
                        <th>
                            <span>Image: </span>
                        </th>
                        <td>
                            <img :src="sourceItem.image">
                        </td>
                        <td>
                            <img :src="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Image url: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.image }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Label: </span>
                        </th>
                        <td>
                            <span><a :href="originalLabels.url">{{ sourceItem.label }}</a></span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.label">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Category: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.categoryLabel }} ({{ sourceItem.category }})</span>
                            <br>
                            <span class="fw-light text-secondary">{{ originalLabels.categoryLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.category">
                                <option disabled>Chose category</option>
                                <option v-for="category in categories" v-bind:value="category.id">{{ category.label }}</option>
                            </select>
                            <button class="btn btn-primary" @click="insertCategory">
                                <i class="bi bi-plus-square"></i>
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Package: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.packageLabel }} ({{ sourceItem.package }})</span>
                            <br>
                            <span class="fw-light text-secondary">{{ originalLabels.packageLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.package">
                                <option disabled>Chose package</option>
                                <option v-for="package in packages" v-bind:value="package.id">{{ package.label }}</option>
                            </select>
                            <button class="btn btn-primary" @click="insertPackage">
                                <i class="bi bi-plus-square"></i>
                            </button>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Brand: </span>
                        </th>
                        <td>
                            <span>{{ sourceLabels.brandLabel }} ({{ sourceItem.brand }})</span>
                            <br>
                            <span class="fw-light text-secondary">{{ originalLabels.brandLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" v-model="destinationItem.brand">
                                <option disabled>Chose brand</option>
                                <option v-for="brand in brands" v-bind:value="brand.id">{{ brand.label }}</option>
                            </select>
                            <a class="btn btn-primary">
                                <i class="bi bi-plus-square"></i>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Consist: </span>
                        </th>
                        <td>
                            <span v-for="consistLabel in sourceLabels.consistLabels">{{ consistLabel }}</span>
                        </td>
                        <td class="input-group">
                            <select class="form-select" multiple v-model="destinationItem.consist">
                                <option disabled>Chose consists</option>
                                <option v-for="consist in consists" v-bind:value="consist.id">{{ consist.label }}</option>
                            </select>
                            <a class="btn btn-primary">
                                <i class="bi bi-plus-square"></i>
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Units: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.units }} {{ sourceLabels.packageShort }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.units">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Term: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.term }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.term">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Calorie: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.calorie }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.calorie">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Carbohydrates: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.carbohydrates }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.carbohydrates">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Fat: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.fat }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.fat">
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <span>Proteins: </span>
                        </th>
                        <td>
                            <span>{{ sourceItem.proteins }}</span>
                        </td>
                        <td class="input-group">
                            <input class="form-control" v-model="destinationItem.proteins">
                        </td>
                    </tr>
                </tbody>
            </table>
            <button class="btn btn-primary" v-on:click='$emit("insert")'></button>
        </div>
    `,
    data() {
        return {
            isCategoryInsertAwailible: false,
            isPackageInsertAwailible: false
        }
    },
    props: {
        sourceItem: {
            type: Object
        },
        destinationItem: {
            type: Object
        },
        originalLabels: {
            type: Object
        }
    },
    methods: {
        insertCategory: function () {
            this.isCategoryInsertAwailible = true;
        },
        categoryInserted: function () {
            this.isCategoryInsertAwailible = false;
        },
        insertPackage: function () {
            this.isPackageInsertAwailible = true;
        },
        packageInserted: function () {
            this.isPackageInsertAwailible = false;
        }
    },
    computed: {
        sourceLabels: function () {
            var itemCategoryLabels = this.$labels.categories.filter(value => value.id == this.sourceItem.category);
            var itemBrandLabels = this.$labels.brands.filter(value => value.id == this.sourceItem.brand);
            var itemPackageLabels = this.$labels.packages.filter(value => value.id == this.sourceItem.package);
            var itemConsistLabels = this.sourceItem.consist !== null && this.sourceItem.consist.length > 0 ? this.$labels.consists.filter(value => this.sourceItem.consist.includes(value.id)) : [];
            var itemCountryLabels = this.$labels.countries.filter(value => value.id == this.sourceItem.additional.country);

            return {
                categoryLabel: itemCategoryLabels.length > 0 ? itemCategoryLabels[0].label : null,
                brandLabel: itemBrandLabels.length > 0 ? itemBrandLabels[0].label : null,
                packageLabel: itemPackageLabels.length > 0 ? itemPackageLabels[0].label : null,
                consistLabels: itemConsistLabels.length > 0 ? itemConsistLabels.map(value => value.label) : [],
                countryLabel: itemCountryLabels.length > 0 ? itemCountryLabels[0].label : null,
                packageShort: itemPackageLabels.length > 0 ? itemPackageLabels[0].short : null,
            };
        },
        categories: {
            cache: false,
            get: function () {
                return this.$labels.categories;
            }
        },
        packages: {
            cache: false,
            get: function () {
                return this.$labels.packages;
            }
        },
        brands: {
            cache: false,
            get: function () {
                return this.$labels.brands;
            }
        },
        consists: {
            cache: false,
            get: function () {
                return this.$labels.consists;
            }
        }
    }
});


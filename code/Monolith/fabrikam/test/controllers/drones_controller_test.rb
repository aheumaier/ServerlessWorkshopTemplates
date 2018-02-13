require 'test_helper'

class DronesControllerTest < ActionDispatch::IntegrationTest
  setup do
    @drone = drones(:one)
  end

  test "should get index" do
    get drones_url
    assert_response :success
  end

  test "should get new" do
    get new_drone_url
    assert_response :success
  end

  test "should create drone" do
    assert_difference('Drone.count') do
      post drones_url, params: { drone: { has_load: @drone.has_load, state: @drone.state } }
    end

    assert_redirected_to drone_url(Drone.last)
  end

  test "should show drone" do
    get drone_url(@drone)
    assert_response :success
  end

  test "should get edit" do
    get edit_drone_url(@drone)
    assert_response :success
  end

  test "should update drone" do
    patch drone_url(@drone), params: { drone: { has_load: @drone.has_load, state: @drone.state } }
    assert_redirected_to drone_url(@drone)
  end

  test "should destroy drone" do
    assert_difference('Drone.count', -1) do
      delete drone_url(@drone)
    end

    assert_redirected_to drones_url
  end
end
